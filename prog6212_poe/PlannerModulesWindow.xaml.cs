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

namespace prog6212_poe
{
    /// <summary>
    /// Interaction logic for PlannerModulesWindow.xaml
    /// </summary>
    public partial class PlannerModulesWindow : Window
    {
        public PlannerModulesWindow()
        {
            InitializeComponent();
            List<string> test2 = new List<string>() { "a", "b", "c", "d" };
            this.modulesListView.ItemsSource = test2;
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
        }
        //dnd Disable The Window Close Button

        private void selectModuleButton_Click(object sender, RoutedEventArgs e)
        {
            Window moduleDetailsWindow = new PlannerModuleViewWindow();
            moduleDetailsWindow.Show();
            this.Close();
        }

        private void returnToSemesterViewButton_Click(object sender, RoutedEventArgs e)
        {
            Window viewSemestersWindow = new PlannerSemestersWindow();
            viewSemestersWindow.Show();
            this.Close();
        }
    }
}
