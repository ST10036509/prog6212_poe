using HoursForYourLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace prog6212_poe
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            //open main window window
            Window mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void registerButton_Click(object sender, MouseButtonEventArgs e)
        {
            //open register window
            Window registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private void exitProgramButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

    }
}
