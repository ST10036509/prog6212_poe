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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //carry over variables:
        private List<Semester> semesters = new List<Semester>();

        //----------------------------------------------------------------------------------------------Constuctors

        //Constructor
        public MainWindow()
        {
            InitializeComponent();
        }//end constructor

        //OVERLAODED constructor
        public MainWindow(List<Semester> semesters)
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
        //End Disable The Window Close Button

        //----------------------------------------------------------------------------------------------AddSemesterButton_Click

        //open semester page
        private void AddSemesterButton_Click(object sender, RoutedEventArgs e)
        {
            Window createSemesterWindow = new SemesterCreationWindow(semesters);
            createSemesterWindow.Show();
            this.Close();
        }//end AddSemesterButton_Click method

        //----------------------------------------------------------------------------------------------PlannerBookButton_Click

        //open planner page
        private void PlannerBookButton_Click(object sender, RoutedEventArgs e)
        {
            //check if there are any semesters
            if (semesters.Count() >= 1)
            {
                //open planner page
                Window viewSemestersWindow = new PlannerSemestersWindow(semesters);
                viewSemestersWindow.Show();
                this.Close();
            }
            else
            {
                //error message
                 MessageBox.Show("Please make sure you create at least ONE semester before proceeding!", "HoursForYou");
            }
        }//end PlannerBookButton_Click method

        //----------------------------------------------------------------------------------------------ExitProgramButton_Click

        //close program
        private void ExitProgramButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }//end exitProgramButton_Click method
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________