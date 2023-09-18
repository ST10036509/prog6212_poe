﻿using System;
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
    /// Interaction logic for PlannerModulesWindow.xaml
    /// </summary>
    public partial class PlannerModulesWindow : Window
    {
        //carry over variables:
        List<Semester> semesters = new List<Semester>();
        List<Module> modules = new List<Module>();

        //Constructor
        public PlannerModulesWindow()
        {
            InitializeComponent();
        }//end constructor

        //OVERLOADED constructor
        public PlannerModulesWindow(List<Semester> semesters, List<Module> modules)
        {
            InitializeComponent();
            this.semesters = semesters;
            this.modules = modules;
            DisplayModules();
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
        //end Disable The Window Close Button

        //display the modules in the list view
        public void DisplayModules()
        {
            //local variable declarations:
            //find list of names:
            var moduleNames = modules.Select(module => module.ModuleName).ToList();

            //display names
            foreach (String name in moduleNames)
            {
                modulesListView.Items.Add(name);
            }
        }//end DisplayModules method

        //select a module && go to the module details window
        private void selectModuleButton_Click(object sender, RoutedEventArgs e)
        {
            //check if no module has been selected
            if (!(modulesListView.SelectedIndex >= 0))
            {
                MessageBox.Show("No Module Selected!");
                return;
            }

            //open module view window
            Window moduleDetailsWindow = new PlannerModuleViewWindow(semesters, modules, modules[modulesListView.SelectedIndex]);
            moduleDetailsWindow.Show();
            this.Close();
        }//end selectModuleButton_Click method

        //return to the semester view window
        private void returnToSemesterViewButton_Click(object sender, RoutedEventArgs e)
        {
            Window viewSemestersWindow = new PlannerSemestersWindow(semesters);
            viewSemestersWindow.Show();
            this.Close();
        }//end returnToSemesterViewButton_Click method
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________