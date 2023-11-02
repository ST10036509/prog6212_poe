using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using HoursForYourLib;

namespace prog6212_poe
{
    /// <summary>
    /// Interaction logic for PlannerModuleViewWindow.xaml
    /// </summary>
    public partial class PlannerModuleViewWindow : Window
    {
        //carry over variables:
        Module selectedModule;
        int weeks = 0;
        int moduleID;
        int semesterID;
        readonly SqlConnection cnn;

        //----------------------------------------------------------------------------------------------Constructors

        //Constructor
        public PlannerModuleViewWindow()
        {
            InitializeComponent();
        }//end constructor

        //OVERLOADED DB Constructor
        public PlannerModuleViewWindow(int moduleID, int semesterID)
        {
            InitializeComponent();

            //establish database connection string
            var connectionString = App.Current.Properties["ConnectionString"] as String;

            //create a connection usning connection string
            cnn = new SqlConnection(connectionString);
            //open the connection
            cnn.Open();

            this.moduleID = moduleID;
            this.semesterID = semesterID;

            //display module data
            DisplayModuleData();
        }//end constructor

        //----------------------------------------------------------------------------------------------Remove Exit Button

        //Disable The Window Close Button
        //import .ddl files for the windows api
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        //declare variables for attributes
        private const uint MF_BYCOMMAND = 0x00000000;
        private const uint MF_GRAYED = 0x00000001;
        private const uint SC_CLOSE = 0xF060;
        private const int WM_SHOWWINDOW = 0x00000018;

        //override the on source initialized method
        protected override void OnSourceInitialized(EventArgs e)
        {
            //call the base on source initialized method
            base.OnSourceInitialized(e);
            //get the window handle
            var hWnd = new WindowInteropHelper(this);
            //get the system menu
            var sysMenu = GetSystemMenu(hWnd.Handle, false);
            //disable the menu item
            EnableMenuItem(sysMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
        }//end Disable The Window Close Button

        //----------------------------------------------------------------------------------------------DisplayModuleData

        //method to display module data
        public async void DisplayModuleData()
        {
            //get the module data
            selectedModule = await Task.Run(() => GetModuleByIndex());

            //display module data
            moduleNameTextBlock.Text = selectedModule.ModuleName;
            moduleCodeTextBlock.Text = selectedModule.ModuleCode;
            creditsTextBlock.Text = selectedModule.Credits.ToString();

            //display module data
            foreach (KeyValuePair<string, double> week in selectedModule.CompletedHours)
            {
                weekComboBox.Items.Add(int.Parse(week.Key) + 1);
                ++weeks;
            }
            //default selection to first item/week
            weekComboBox.SelectedIndex = 0;
        }//end DisplayModuleData method

        //----------------------------------------------------------------------------------------------GetModuleByIndex

        public async Task<Module> GetModuleByIndex()
        {
            //cretae a module datamodel
            Module module = new Module();
            //sql query to get the module data
            string query = "SELECT ModuleName, ModuleCode, NumberOfCredits, NumberOfHoursPerWeek, StartDate, SelfStudyHours, CompletedHours FROM Modules WHERE ModuleID = @ModuleID";
            //create sql command and set paramter values
            SqlCommand command = new SqlCommand(query, cnn);
            command.Parameters.AddWithValue("@ModuleID", moduleID);

            //use a sqlreader to capture the module data and place it into a datamodel
            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    module.ModuleName = (string)reader["ModuleName"];
                    module.ModuleCode = (string)reader["ModuleCode"];
                    module.Credits = (int)reader["NumberOfCredits"];
                    module.ClassHours = (int)reader["NumberOfHoursPerWeek"];
                    module.SemesterStartDate = (DateTime)reader["StartDate"];
                    module.SelfStudyHours = (int)reader["SelfStudyHours"];
                        
                    //convert the json string to a dictionary<string, double>
                    string jsonString = reader["CompletedHours"].ToString();
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    dynamic jsonObject = serializer.Deserialize<dynamic>(jsonString);
                    module.CompletedHours = serializer.ConvertToType<Dictionary<string,double>>(jsonObject);
                }
            }
            //return the datamodel
            return module;
        }//end GetSelectedModuleData method

        //----------------------------------------------------------------------------------------------addHoursButton_Click

        //add hours to the selected week
        public async void addHoursButton_Click(object sender, RoutedEventArgs e)
        {
            //local variable declarations:
            double hours = 0;
            bool hoursIsParsable = Double.TryParse(hoursTextBox.Text, out hours);
            DateTime date;
            bool dateIsParsable = DateTime.TryParse(selectedDateDatePicker.Text, out date);
            
            //check if hours is valid
            if (!hoursIsParsable)
            {
                //error message if hours is not valid
                MessageBox.Show("Please Ensure Hours Is A Valid Number!");
                return;
            }
            
            //check if date is valid
            if (!dateIsParsable)
            {
                //error message if date is not valid
                MessageBox.Show("Please Ensure Date Is A Valid Date!");
                return;
            }

            //check if hours and date are null
            if ((hoursTextBox.Text == "") || (selectedDateDatePicker.Text == ""))
            {
                //error message if either are null
                MessageBox.Show("Please Ensure You Fill Out All Data Fields!");
                return;
            }

            //check if hour is positive
            if (hours < 0)
            {
                //error message if hours is negative
                MessageBox.Show("Please Ensure Hours Is A POSITIVE Number!");
                return;
            }

            //run method to update the hours in the spceified date/week
            HoursForYourLib.Calculations calc = new Calculations();
            var output = calc.IdentifyAndUpdateWeek(date, selectedModule, hoursTextBox.Text);

            //update the hours text block
            hoursCompletedTextBlock.Text = selectedModule.CompletedHours[output.Key.ToString()] + "/" + selectedModule.SelfStudyHours.ToString();

            //update the database
            await Task.Run(() => UpdateCompletedHours());

            //reset the inputs
            selectedDateDatePicker.Text = null;
            hoursTextBox.Text = null;

            //update visability
            messageTextBlock.Visibility = Visibility.Visible;
        }//end addHoursButton_Click method

        //----------------------------------------------------------------------------------------------UpdateCompletedHours

        public async Task UpdateCompletedHours()
        {
            //convert the dictionary to a json string
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var jsonCompletedHours = serializer.Serialize(selectedModule.CompletedHours);

            //sql query to update  selected module data
            string query = "UPDATE Modules SET CompletedHours = @CompletedHours WHERE ModuleID = @ModuleID;";
            //create sql command and set paramter values
            SqlCommand command = new SqlCommand(query, cnn);
            command.Parameters.AddWithValue("@ModuleID", moduleID);
            command.Parameters.AddWithValue("@CompletedHours", jsonCompletedHours);

            //execute the query
            await command.ExecuteNonQueryAsync();
        }//end UpdateModuleDatabase method

        //----------------------------------------------------------------------------------------------weekComboBox_SelectionChanged

        //method to update when a new week is selected
        public void weekComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //declare variable to hold selectedWeek
            var selectedWeek = Int32.Parse(weekComboBox.SelectedItem.ToString()) - 1;

            //display hours completed
            hoursCompletedTextBlock.Text = selectedModule.CompletedHours[selectedWeek.ToString()] + "/" + selectedModule.SelfStudyHours.ToString();

            //set the datepicker limits to the selected week start and end dates
            selectedDateDatePicker.DisplayDateStart = selectedModule.SemesterStartDate.AddDays(7 * selectedWeek);
            var currentStartDate = selectedDateDatePicker.DisplayDateStart.Value;
            selectedDateDatePicker.DisplayDateEnd = currentStartDate.AddDays(6);

            //update message visability
            messageTextBlock.Visibility = Visibility.Hidden;
        }//end weekComboBox_SelectionChanged method

        //----------------------------------------------------------------------------------------------_GotFocus

        ////reset after hours are added:
        public void _GotFocus(object sender, RoutedEventArgs e)
        {
            //update visability
            messageTextBlock.Visibility = Visibility.Hidden;
        }//end _GotFocus method

        //----------------------------------------------------------------------------------------------returnToModulesViewButton_Click

        //return to modules view page
        public void returnToModulesViewButton_Click(object sender, RoutedEventArgs e)
        {
            //close connection
            cnn.Close();

            //open planner module window
            Window viewModulesWindow = new PlannerModulesWindow(semesterID);
            viewModulesWindow.Show();
            this.Close();
        }//end returnToModulesViewButton_Click method
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________