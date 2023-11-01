using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        List<Semester> semesters = new List<Semester>();
        List<Module> modules = new List<Module>();
        Module selectedModule;
        int weeks = 0;
        int moduleID;
        int semesterID;
        readonly private SqlConnection cnn;

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

            DisplayModuleData();
        }//end constructor


        ////OVERLOADED constructor
        //public PlannerModuleViewWindow(List<Semester> semesters, List<Module> modules, Module selectedModule)
        //{
        //    InitializeComponent();
        //    this.semesters = semesters;
        //    this.modules = modules;
        //    this.selectedModule = selectedModule;
        //    DisplayModuleData();
        //}//end OVERLOADED constructor

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
            var selectedModule = await Task.Run(() => GetModuleByIndex());

            //display module data
            moduleNameTextBlock.Text = selectedModule.ModuleName;
            moduleCodeTextBlock.Text = selectedModule.ModuleCode;
            creditsTextBlock.Text = selectedModule.Credits.ToString();

            //display semester data
            //foreach (KeyValuePair<int, double> week in selectedModule.CompletedHours)
            //{
            //    weekComboBox.Items.Add(week.Key + 1);
            //    ++weeks;
            //}
            //default selection to first item/week
            weekComboBox.SelectedIndex = 0;
        }//end DisplayModuleData method

        public async Task<Module> GetModuleByIndex()
        {
            Module module = new Module();

            using (cnn)
            {
                string query = "SELECT ModuleName, ModuleCode, NumberOfCredits, NumberOfHoursPerWeek, StartDate, SelfStudyHours, CompletedHours FROM Modules WHERE ModuleID = @ModuleID";
                SqlCommand command = new SqlCommand(query, cnn);
                command.Parameters.AddWithValue("@ModuleID", moduleID);

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
                        
                        //string jsonString = reader["CompletedHours"].ToString();
                        //JavaScriptSerializer serializer = new JavaScriptSerializer();
                        //dynamic jsonObject = serializer.Deserialize<dynamic>(jsonString);
                        //Dictionary<string, double> dictionary = serializer.ConvertToType<Dictionary<string,double>>(jsonObject);
                        //Dictionary<int, double> intDictionary = dictionary.ToDictionary(pair => int.Parse(pair.Key), pair => pair.Value);
                        //module.CompletedHours = dictionary;


                        //var moduleName = (string)reader["ModuleName"];
                        //var moduleCode = (string)reader["ModuleCode"];
                        //var numberOfCredits = (int)reader["NumberOfCredits"];
                        //var numberOfHoursPerWeek = (int)reader["NumberOfHoursPerWeek"];
                        //var startDate = (DateTime)reader["StartDate"];
                        //var selfStudyHours = (int)reader["SelfStudyHours"];
                        //var completedHours = (string)reader["CompletedHours"];

                    }
                }
            }

            return module;
        }//end GetSelectedModuleData method

        //----------------------------------------------------------------------------------------------addHoursButton_Click

        //add hours to the selected week
        private void addHoursButton_Click(object sender, RoutedEventArgs e)
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
            hoursCompletedTextBlock.Text = selectedModule.CompletedHours[output.Key] + "/" + selectedModule.SelfStudyHours.ToString();

            //reset the inputs
            selectedDateDatePicker.Text = null;
            hoursTextBox.Text = null;

            //update visability
            messageTextBlock.Visibility = Visibility.Visible;
            //IdentifyAndUpdateWeek(date);
        }//end addHoursButton_Click method

        //----------------------------------------------------------------------------------------------weekComboBox_SelectionChanged

        //method to update when a new week is selected
        private void weekComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //declare variable to hold selectedWeek
            var selectedWeek = Int32.Parse(weekComboBox.SelectedItem.ToString()) - 1;

            //display hours completed
            hoursCompletedTextBlock.Text = selectedModule.CompletedHours[selectedWeek] + "/" + selectedModule.SelfStudyHours.ToString();

            //set the datepicker limits to the selected week start and end dates
            selectedDateDatePicker.DisplayDateStart = selectedModule.SemesterStartDate.AddDays(7 * selectedWeek);
            var currentStartDate = selectedDateDatePicker.DisplayDateStart.Value;
            selectedDateDatePicker.DisplayDateEnd = currentStartDate.AddDays(6);

            //update message visability
            messageTextBlock.Visibility = Visibility.Hidden;
        }//end weekComboBox_SelectionChanged method

        //----------------------------------------------------------------------------------------------_GotFocus

        ////reset after hours are added:
        private void _GotFocus(object sender, RoutedEventArgs e)
        {
            //update visability
            messageTextBlock.Visibility = Visibility.Hidden;
        }//end _GotFocus method

        //----------------------------------------------------------------------------------------------returnToModulesViewButton_Click

        //return to modules view page
        private void returnToModulesViewButton_Click(object sender, RoutedEventArgs e)
        {
            //open planner module window
            Window viewModulesWindow = new PlannerModulesWindow(semesterID);
            viewModulesWindow.Show();
            this.Close();
        }//end returnToModulesViewButton_Click method
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________