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
        int weeks = 0;

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
            foreach (KeyValuePair<int, double> week in selectedModule.CompletedHours)
            {
                weekComboBox.Items.Add(week.Key + 1);
                ++weeks;
            }
            weekComboBox.SelectedIndex = 0;
        }

        private void addHoursButton_Click(object sender, RoutedEventArgs e)
        {
            double hours = 0;
            bool hoursIsParsable = Double.TryParse(hoursTextBox.Text, out hours);
            DateTime date;
            bool dateIsParsable = DateTime.TryParse(selectedDateDatePicker.Text, out date);
            
            if (!hoursIsParsable)
            {
                MessageBox.Show("Please Ensure Hours Is A Valid Number!");
                return;
            }
            
            if (!dateIsParsable)
            {
                MessageBox.Show("Please Ensure Date Is A Valid Date!");
                return;
            }

            if ((hoursTextBox.Text == "") || (selectedDateDatePicker.Text == ""))
            {
                MessageBox.Show("Please Ensure You Fill Out All Data Fields!");
                return;
            }

            //IdentifyWeek(date);
            //UpdateWeek(0);
        }

        //public void IdentifyWeek(DateTime date)
        //{
        //    double dayIndex = (date.Subtract(selectedModule.SemesterStartDate).Days) / 7;

        //    foreach (KeyValuePair<int, double> week in selectedModule.CompletedHours)
        //    {
        //        int count = selectedModule.CompletedHours.Count();

        //        if (dayIndex >= week.Key + 1)
        //        {
        //            break;
        //        }
        //        else
        //        {

        //        }

        //    }
        //}

        public void UpdateWeek(int week)
        {
            selectedModule.CompletedHours[week] = Double.Parse(hoursTextBox.Text);
        }

        private void weekComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedWeek = Int32.Parse(weekComboBox.SelectedItem.ToString()) - 1;

            hoursCompletedTextBlock.Text = selectedModule.CompletedHours[selectedWeek] + "/" + selectedModule.SelfStudyHours.ToString();
            selectedDateDatePicker.DisplayDateStart = selectedModule.SemesterStartDate.AddDays(7 * selectedWeek);

            var currentStartDate = selectedDateDatePicker.DisplayDateStart.Value;
            selectedDateDatePicker.DisplayDateEnd = currentStartDate.AddDays(6);
        }

        //return to modules view page
        private void returnToModulesViewButton_Click(object sender, RoutedEventArgs e)
        {
            Window viewModulesWindow = new PlannerModulesWindow(semesters, modules);
            viewModulesWindow.Show();
            this.Close();
        }//end returnToModulesViewButton_Click method
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________