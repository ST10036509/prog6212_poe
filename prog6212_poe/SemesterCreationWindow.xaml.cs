using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        //carry over variables:
        private List<Semester> semesters = new List<Semester>();
        private List<Module> modules = new List<Module>();
        private string semesterName = "";
        private double semesterNumberOfWeeks = 0;
        private DateTime semesterStartDate = DEFAULT_DATE;

        //----------------------------------------------------------------------------------------------Constructors

        //constructor
        public SemesterCreationWindow()
        {
            InitializeComponent();
        }//end constructor

        //OVERLOADED constructor
        public SemesterCreationWindow(List<Semester> semesters, List<Module> modules, string name, double weeks, DateTime date)
        {
            InitializeComponent();
            this.semesters = semesters;
            this.modules = modules;
            this.semesterName = name;
            this.semesterNumberOfWeeks = weeks;
            this.semesterStartDate = date;
            FillTextBoxData();

        }//end OVERLOADED constructor

        //OVERLOADED constructor
        public SemesterCreationWindow(List<Semester> semesters)
        {
            InitializeComponent();
            this.semesters = semesters;
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

            //open add module window
            Window addModuleWindow = new AddModuleWindow(semesters, modules, semesterName, semesterNumberOfWeeks, semesterStartDate);
            addModuleWindow.Show();
            this.Close();
        }//end AddModleButton_Click method

        //----------------------------------------------------------------------------------------------CreateSemesterButton_Click

        //open create semester page
        private void CreateSemesterButton_Click(object sender, RoutedEventArgs e)
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
                                //add semester to list
                                semesters.Add(newSemester);

                                //clear TextBox values
                                semesterNameTextBox.Text = "";
                                numberOfWeeksTextBox.Text = "";
                                startDateDatePicker.Text = "";

                                //display success message
                                messageTextBlock.Visibility = Visibility.Visible;

                                //open main window
                                Window mainWindow = new MainWindow(semesters);
                                mainWindow.Show();
                                this.Close();
                            }
                        }
                    }
                }
            }
        }//end CreateSemesterButton_Click method

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
            //open main window
            Window mainWindow = new MainWindow(semesters);
            mainWindow.Show();
            this.Close();
        }//end ReturnToMainMenuButton_Click method
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________