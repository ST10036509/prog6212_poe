﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using HoursForYourLib;

namespace prog6212_poe
{
    /// <summary>
    /// Interaction logic for AddModuleWindow.xaml
    /// </summary>
    public partial class AddModuleWindow : Window
    {
        //carry over variables:
        private List<Module> modules = new List<Module>();
        private string semesterName;
        private double numberOfWeeks;
        private DateTime startDate;

        //----------------------------------------------------------------------------------------------Constructors

        //Constructor
        public AddModuleWindow()
        {
            InitializeComponent();
        }//end constructor

        //OVERLOADED Constructor (CREATE SEMESTER >> ADD MODULE)
        public AddModuleWindow(List<Module> modules,
                               string name, double weeks, DateTime date)
        {
            InitializeComponent();
            this.modules = modules;
            this.semesterName = name;
            this.numberOfWeeks = weeks;
            this.startDate = date;
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

        //----------------------------------------------------------------------------------------------AddModuleButton_Click

        //add module and return to create semester page
        private void AddModuleButton_Click(object sender, RoutedEventArgs e)
        {
            //declare variables
            double credits = 0;
            bool moduleCreditsIsParsable = double.TryParse(numberOfCreditsTextBox.Text, out credits);
            double hours = 0;
            bool moduleHoursIsParsable = double.TryParse(classHoursPerWeekTextBox.Text, out hours);

            //update success message visability
            messageTextBlock.Visibility = Visibility.Hidden;

            //validate input is not null
            if (moduleNameTextBox.Text == "" || moduleCodeTextBox.Text == "" || numberOfCreditsTextBox.Text == "" || classHoursPerWeekTextBox.Text == "")
            {
                //error message if fields are empty
                //display error message
                messageTextBlock.Text = "Please Fill In All The Fields!";
                messageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                messageTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                //validate input for credits
                if (moduleCreditsIsParsable == false)
                {
                    //error message if credits is not a number
                    //display error message
                    messageTextBlock.Text = "CREDITS Must Be A Valid Number!";
                    messageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                    messageTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    //validate input for hours
                    if (moduleHoursIsParsable == false)
                    {
                        //error message if hours is not a number
                        //display error message
                        messageTextBlock.Text = "HOURS Must Be A Valid Number!";
                        messageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                        messageTextBlock.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (hours < 0)
                        {
                            //error message if hours is not a positive number
                            //display error message
                            messageTextBlock.Text = "HOURS Must Be A POSITIVE!";
                            messageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                            messageTextBlock.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            if (credits < 0)
                            {
                                //error message if credits is not a positive number
                                //display error message
                                messageTextBlock.Text = "CREDIT MUST BE POSITIVE!";
                                messageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                                messageTextBlock.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                //create new module
                                Module newModule = new Module(moduleNameTextBox.Text, moduleCodeTextBox.Text, credits, hours, startDate);
                                //add module to list
                                modules.Add(newModule);

                                //clear fields
                                moduleNameTextBox.Text = "";
                                moduleCodeTextBox.Text = "";
                                numberOfCreditsTextBox.Text = "";
                                classHoursPerWeekTextBox.Text = "";

                                //display success message
                                messageTextBlock.Text = "Successfully Added A Module!";
                                messageTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                                messageTextBlock.Visibility = Visibility.Visible;
                            }
                        }
                    }
                }
            }   
        }//end addModuleButton_Click method

        //----------------------------------------------------------------------------------------------_GotFocus

        //reset after creation:
        private void _GotFocus(object sender, RoutedEventArgs e)
        {
            //update visability
            messageTextBlock.Visibility = Visibility.Hidden;
        }//end _GotFocus method

        //----------------------------------------------------------------------------------------------ReturnToCreateSemesterButton_Click

        //return to create semester page
        private void ReturnToCreateSemesterButton_Click(object sender, RoutedEventArgs e)
        {
            //open semester creation window
            Window createSemesterWindow = new SemesterCreationWindow(modules, semesterName, numberOfWeeks, startDate);
            createSemesterWindow.Show();
            this.Close();
        }//end returnToCreateSemesterButton_Click method
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________