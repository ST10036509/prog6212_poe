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
    /// Interaction logic for SemesterCreationWindow.xaml
    /// </summary>
    public partial class SemesterCreationWindow : Window
    {
        //carry over variables:
        private List<Semester> semesters = new List<Semester>();
        private List<Module> modules = new List<Module>();


        //constructor
        public SemesterCreationWindow()
        {
            InitializeComponent();
        }//end constructor

        //OVERLOADED Constructor
        public SemesterCreationWindow(List<Semester> semesters, List<Module> modules)
        {
            this.semesters = semesters;
            this.modules = modules;
            InitializeComponent();
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

        //open add module page
        private void AddModleButton_Click(object sender, RoutedEventArgs e)
        {
            Window addModuleWindow = new AddModuleWindow(semesters, modules);
            addModuleWindow.Show();
            this.Close();
        }//end AddModleButton_Click method

        //open create semester page
        private void CreateSemesterButton_Click(object sender, RoutedEventArgs e)
        {
            Window mainWindow = new MainWindow(semesters, modules);
            mainWindow.Show();
            this.Close();
        }//end CreateSemesterButton_Click method

        //return to main home page
        private void ReturnToMainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            Window mainWindow = new MainWindow(semesters, modules);
            mainWindow.Show();
            this.Close();
        }//end ReturnToMainMenuButton_Click method
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________