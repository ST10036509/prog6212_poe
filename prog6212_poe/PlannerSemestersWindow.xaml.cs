using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HoursForYourLib;
using static prog6212_poe.PlannerModulesWindow;

namespace prog6212_poe
{
    /// <summary>
    /// Interaction logic for PlannerSemestersWindow.xaml
    /// </summary>
    public partial class PlannerSemestersWindow : Window
    {
        //carry over variables:
        List<Semester> semesters = new List<Semester>();

        //Constructor
        public PlannerSemestersWindow()
        {
            InitializeComponent();
            DisplaySemesters();
        }//end constructor

        //OVERLOADED constructor
        public PlannerSemestersWindow(List<Semester> semesters)
        {
            InitializeComponent();
            this.semesters = semesters;
            DisplaySemesters();
        }//end OVERLOADED constructor

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

        public void DisplaySemesters()        
        {
            var myItems = semesters.Select(semester => new MySemesterItem
            {
                Name = semester.SemesterName,
            }).ToList();

            semestersListView.ItemsSource = myItems;
        }
        //end DisplaySemsters method 
        public class MySemesterItem
        {
            public string Name { get; set; }
        }
        //select a semester && open the modules selection window
        private void selectSemesterButton_Click(object sender, RoutedEventArgs e)
        {
            //check if no semester has been selected
            if (!(semestersListView.SelectedIndex >= 0))
            {
                MessageBox.Show("No Semester Selected!");
                return;
            }

            //fetch the index of the selected semester
            var index = semestersListView.SelectedIndex;

            //open module selection window
            Window viewModulesWindow = new PlannerModulesWindow(semesters, semesters[index].Modules);
            viewModulesWindow.Show();
            this.Close();
        }//end selectSemesterButton_Click method

        //return to Main Menu page
        private void returnToMainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            Window mainWindow = new MainWindow(semesters);
            mainWindow.Show();
            this.Close();
        }//end returnToMainMenuButton_Click method
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________