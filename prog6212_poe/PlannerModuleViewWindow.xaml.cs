using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;
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

        //Constructor
        public PlannerModuleViewWindow()
        {
            InitializeComponent();
        }//end constructor


        //OVERLOADED constructor
        public PlannerModuleViewWindow(List<Semester> semesters, List<Module> modules, Module selectedModule)
        {
            InitializeComponent();
            this.semesters = semesters;
            this.modules = modules;
            this.selectedModule = selectedModule;
            DisplayModuleData();
        }
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

        public void DisplayModuleData()
        {
            moduleNameTextBlock.Text = selectedModule.ModuleName;
            moduleCodeTextBlock.Text = selectedModule.ModuleCode;
            creditsTextBlock.Text = selectedModule.Credits.ToString();
            hoursPerWeekTextBlock.Text = selectedModule.SelfStudyHours.ToString();
            hoursCompletedTextBlock.Text = "0/" + selectedModule.SelfStudyHours.ToString();
        }

        //return to modules view page
        private void returnToModulesViewButton_Click(object sender, RoutedEventArgs e)
        {
            Window viewModulesWindow = new PlannerModulesWindow(semesters, modules);
            viewModulesWindow.Show();
            this.Close();
        }//end returnToModulesViewButton_Click method

        private void addHoursButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________