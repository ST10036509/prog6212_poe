using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Interop;
using HoursForYourLib;

namespace prog6212_poe
{
    /// <summary>
    /// Interaction logic for SemesterCreationWindow.xaml
    /// </summary>
    public partial class SemesterCreationWindow : Window
    {
        //CONSTANTS
        static DateTime DEFAULT_DATE = DateTime.Today;
        readonly private SqlConnection cnn;

        //carry over variables:
        private List<Module> modules = new List<Module>();
        private string semesterName = "";
        private double semesterNumberOfWeeks = 0;
        private DateTime semesterStartDate = DEFAULT_DATE;
        int userID;

        //----------------------------------------------------------------------------------------------Constructors

        //constructor (MAIN WINDOW >> CREATE SEMESTER)
        public SemesterCreationWindow()
        {
            InitializeComponent();

            //fetch logged in userid
            userID = (int)App.Current.Properties["UserID"];

            //establish database connection string
            var connectionString = App.Current.Properties["ConnectionString"] as String;

            //create a connection usning connection string
            cnn = new SqlConnection(connectionString);
            //open the connection
            cnn.Open();
        }//end constructor

        //OVERLOADED constructor (ADD MODULE >> CREATE SEMESTER)
        public SemesterCreationWindow(List<Module> modules, string name, double weeks, DateTime date)
        {
            InitializeComponent();

            //fetch logged in userid
            userID = (int)App.Current.Properties["UserID"];

            //establish database connection string
            var connectionString = App.Current.Properties["ConnectionString"] as String;

            //create a connection usning connection string
            cnn = new SqlConnection(connectionString);
            //open the connection
            cnn.Open();

            this.modules = modules;
            this.semesterName = name;
            this.semesterNumberOfWeeks = weeks;
            this.semesterStartDate = date;

            //repopulate textblocks
            FillTextBoxData();
        }//end OVERLOADED constructor

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
        }
        //dnd Disable The Window Close Button

        //----------------------------------------------------------------------------------------------AddModleButton_Click

        //open add module page
        private void AddModleButton_Click(object sender, RoutedEventArgs e)
        {
            //Capture already inputted data to carry over
            CaptureTextBoxData();

            //close connection
            cnn.Close();

            //open add module window
            Window addModuleWindow = new AddModuleWindow(modules, semesterName, semesterNumberOfWeeks, semesterStartDate);
            addModuleWindow.Show();
            this.Close();
        }//end AddModleButton_Click method

        //----------------------------------------------------------------------------------------------CreateSemesterButton_Click

        //open create semester page
        private async void CreateSemesterButton_Click(object sender, RoutedEventArgs e)
        {
            //declare variables
            double weeks;
            //test if weeks is valid double and assign
            bool semesterWeeksIsParsable = double.TryParse(numberOfWeeksTextBox.Text, out weeks);
            DateTime startDate;
            //test if start date is valid datetime and assign
            bool semesterStartDateIsParsable = DateTime.TryParse(startDateDatePicker.Text, out startDate);

            //validate input are not null
            if (semesterNameTextBox.Text == "" || numberOfWeeksTextBox.Text == "" || startDateDatePicker.Text == "")
            {
                //error message if fields are empty
                MessageBox.Show("Please Fill In All The Fields!", "HoursForYou");
            }
            else
            {
                //validate input for weeks
                if (semesterWeeksIsParsable == false)
                {
                    //error message if weeks is not a number
                    MessageBox.Show("Please Make Sure Your Number of Weeks Is A Number!", "HoursForYou");
                }
                else
                {
                    //validate input for start date
                    if (semesterStartDateIsParsable == false)
                    {
                        //error message if start date is not a datetime
                        MessageBox.Show("Please Make Sure Your START DATE Is A Valid Date!", "HoursForYou");
                    }
                    else
                    {
                        //validate there is at LEAST ONE module
                        if (modules.Count() < 1)
                        {
                            //error message if there are no modules
                            MessageBox.Show("Please Make Sure You Add At Least ONE Module Before Proceeding!", "HoursForYou");
                        }
                        else
                        {
                            //validate if the inputted weeks is a positive number
                            if (weeks <= 0)
                            {
                                //error message if weeks is not a positive number
                                MessageBox.Show("Please Make Sure Your Number of Weeks Is A Positive Number And Not ZERO!", "HoursForYou");
                            }
                            else
                            {
                                //calculate and assign the self study hours and generate a dictionary for the weeks with base value of  the self study hours
                                HoursForYourLib.Calculations calc = new Calculations();
                                this.modules = calc.ModuleHoursAssignment(weeks, modules);

                                //create new semester
                                Semester newSemester = new Semester(semesterNameTextBox.Text, weeks, startDate, modules);
                                //add semester to database
                                var semesterID = await Task.Run(() => AddSemesterToDatabase(newSemester));

                                //add moduels to database
                                foreach (Module module in modules)
                                {
                                    await Task.Run(() => AddModuleToDatabase(module, semesterID));
                                }

                                //clear TextBox values
                                semesterNameTextBox.Text = "";
                                numberOfWeeksTextBox.Text = "";
                                startDateDatePicker.Text = "";

                                //display success message
                                messageTextBlock.Visibility = Visibility.Visible;

                                //close connection
                                cnn.Close();

                                //open main window
                                Window mainWindow = new MainWindow();
                                mainWindow.Show();
                                this.Close();
                            }
                        }
                    }
                }
            }
        }//end CreateSemesterButton_Click method

        //----------------------------------------------------------------------------------------------AddSemesterToDatabase

        public async Task<int> AddSemesterToDatabase(Semester newSemester)
        {
            //create sl query to add semester to database
            string query = "INSERT INTO Semesters (UserID, SemesterName, NumberOfWeeks, StartDate) OUTPUT INSERTED.SemesterID VALUES (@UserID, @SemesterName, @NumberOfWeeks, @StartDate);";
            //create command and assign parameter values
            SqlCommand command = new SqlCommand(query, cnn);
            command.Parameters.AddWithValue("@UserID",userID);
            command.Parameters.AddWithValue("@SemesterName", newSemester.SemesterName);
            command.Parameters.AddWithValue("@NumberOfWeeks", newSemester.NumberOfWeeks);
            command.Parameters.AddWithValue("@StartDate", newSemester.StartDate);

            //execute query and return semesterID
            int semesterID = (int)await command.ExecuteScalarAsync();
            return semesterID;
        }//end AddSemesterToDatabase method

        //----------------------------------------------------------------------------------------------AddModuleToDatabase

        public async Task AddModuleToDatabase(Module newModule, int semesterID)
        {
            //serialize the dictionary of completed hours into json string format
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var jsonCompletedHours = serializer.Serialize(newModule.CompletedHours);

            //create sql query to add module to database
            string query = "INSERT INTO Modules (SemesterID, ModuleName, ModuleCode, NumberOfCredits, NumberOfHoursPerWeek, StartDate, SelfStudyHours, CompletedHours) VALUES (@SemesterID, @ModuleName, @ModuleCode, @NumberOfCredits, @NumberOfHoursPerWeek, @StartDate, @SelfStudyHours, @CompletedHours);";
            //create command and assign parameter values
            SqlCommand command = new SqlCommand(query, cnn);
            command.Parameters.AddWithValue("@SemesterID", semesterID);
            command.Parameters.AddWithValue("@ModuleName", newModule.ModuleName);
            command.Parameters.AddWithValue("@ModuleCode", newModule.ModuleCode);
            command.Parameters.AddWithValue("@NumberOfCredits", newModule.Credits);
            command.Parameters.AddWithValue("@NumberOfHoursPerWeek", newModule.ClassHours);
            command.Parameters.AddWithValue("@StartDate", newModule.SemesterStartDate);
            command.Parameters.AddWithValue("@SelfStudyHours", newModule.SelfStudyHours);
            command.Parameters.AddWithValue("@CompletedHours", jsonCompletedHours);

            //execute query
            await command.ExecuteNonQueryAsync();
        }//end AddModuleToDatabase

        //----------------------------------------------------------------------------------------------FillTextBoxData

        //refill textboxes
        public void FillTextBoxData()
        {
            semesterNameTextBox.Text = semesterName;
            numberOfWeeksTextBox.Text = semesterNumberOfWeeks.ToString();
            startDateDatePicker.Text = semesterStartDate.ToString();
        }//end FillTextBoxData method

        //----------------------------------------------------------------------------------------------CaptureTextBoxData

        //Capture GUI Data For Carry Over
        public void CaptureTextBoxData()
        {
            //local variable declaration:
            semesterName = semesterNameTextBox.Text;
            bool weeksIsParsable = Double.TryParse(numberOfWeeksTextBox.Text, out semesterNumberOfWeeks);
            bool startDateIsParsable = DateTime.TryParse(startDateDatePicker.Text, out semesterStartDate);

            //if weeks is an invalid inout then pass a default value
            if (!weeksIsParsable)
            {
                //set default value
                semesterNumberOfWeeks = 1;
            }
            //if start date is an invalid inout then pass a default value
            if (!startDateIsParsable)
            {
                //set default value
                semesterStartDate = DEFAULT_DATE;
            }
        }//end CaptureTextBoxData method

        //----------------------------------------------------------------------------------------------_GotFocus

        //reset after creation:
        private void _GotFocus(object sender, RoutedEventArgs e)
        {
            //update visability
            messageTextBlock.Visibility = Visibility.Hidden;
        }//end _GotFocus method

        //----------------------------------------------------------------------------------------------ReturnToMainMenuButton_Click

        //return to main home page
        private void ReturnToMainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            //close connection
            cnn.Close();

            //open main window
            Window mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }//end ReturnToMainMenuButton_Click method
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________