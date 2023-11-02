using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data.SqlClient;
using System.Security.Cryptography;
using HoursForYourLib;

namespace prog6212_poe
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        //declare GLOBAL variables:
        readonly private SqlConnection cnn;
        private bool usernameExists;

        //----------------------------------------------------------------------------------------------RegisterWindow

        //Constructor
        public RegisterWindow()
        {
            InitializeComponent();

            //establish database connection string
            var connectionString = App.Current.Properties["ConnectionString"] as String;

            //create a connection usning connection string
            cnn = new SqlConnection(connectionString);
            //open the connection
            cnn.Open();
        }//end constructor method

        //----------------------------------------------------------------------------------------------CheckIfUsernameExists

        public async Task<bool> CheckIfUsernameExists(string username)
        {
            //cretae an SQL query to get the number of rows with the username that macth the current user entered value
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username;";

            //create an sql command
            SqlCommand command = new SqlCommand(query, cnn);
            //add valus to the query
            command.Parameters.AddWithValue("@Username", username);

            //check if the username exists
            int count = Convert.ToInt32(await command.ExecuteScalarAsync());
            return count > 0;//return true if the username exists and false if it doesnt
        }//end CheckIfUsernameExists method

        //----------------------------------------------------------------------------------------------usernameTextBox_TextChanged

        private async void usernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //capture username
            string username = usernameTextBox.Text;

            //check if the username exists in the database as te user types
            usernameExists = await Task.Run(() => CheckIfUsernameExists(username));

            //if it exsists
            if (usernameExists)
            {
                //display error message
                messageTextBlock.Text = "USERNAME Is Unavaliable!";
                messageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                messageTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                //clear error message
                messageTextBlock.Visibility = Visibility.Hidden;
            }
        }//end usernameTextBox_TextChanged method

        //----------------------------------------------------------------------------------------------confirmationPasswordTextBox_TextChanged

        private void confirmationPasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if passwords dont match while typing
            if (confirmationPasswordTextBox.Text != passwordTextBox.Text)
            {
                //display error message
                messageTextBlock.Text = "Your PASSWORDS Dont Match!";
                messageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                messageTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                //clear error message
                messageTextBlock.Visibility = Visibility.Hidden;
            }
        }//end confirmationPasswordTextBox_TextChanged method

        //----------------------------------------------------------------------------------------------registerButton_Click

        private async void registerButton_Click(object sender, RoutedEventArgs e)
        {
            //if username exists
            if ( (usernameExists) )
            {
                //display error message
                messageTextBlock.Text = "USERNAME Is Unavaliable!";
                messageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                messageTextBlock.Visibility = Visibility.Visible;
            }
            //if passwords dont match
            else if ( (confirmationPasswordTextBox.Text != passwordTextBox.Text) )
            {
                //display error message
                messageTextBlock.Text = "Your PASSWORDS Dont Match!";
                messageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                messageTextBlock.Visibility = Visibility.Visible;
            }
            //if username is empty
            else if ( (usernameTextBox.Text == "") )
            {
                //display error message
                messageTextBlock.Text = "USERNAME Cannot Be Empty!";
                messageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                messageTextBlock.Visibility = Visibility.Visible;
            }
            //if passwords are empty
            else if ( (passwordTextBox.Text == "") || (confirmationPasswordTextBox.Text == ""))
            {
                //display error messsage
                messageTextBlock.Text = "PASSWORD Cannot Be Empty!";
                messageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                messageTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                //capture username and password
                string username = usernameTextBox.Text;
                string password = passwordTextBox.Text;

                //encrypt password
                PasswordHandler handler = new PasswordHandler();
                var hashedPassword = await Task.Run(() => handler.EncryptPassword(password));

                //add user to database
                var userID = await Task.Run(() => AddUser(username, hashedPassword));

                App.Current.Properties["UserID"] = userID;

                //close database connection
                cnn.Close();

                //open main window 
                Window mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }//end registerButton_Click method

        //----------------------------------------------------------------------------------------------AddUser

        public async Task<int> AddUser(string username, string password)
        {
            //create insert query
            string query = "INSERT INTO Users (Username, [Password]) OUTPUT INSERTED.UserID VALUES (@Username, @Password);";
            //make a new command
            SqlCommand command = new SqlCommand(query, cnn);
            //insert variable values into query
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);

            //run query and get the inserted records UserID
            int userID = (int)await command.ExecuteScalarAsync();
            return userID;
        }//end AddUser method

        //----------------------------------------------------------------------------------------------returnToLoginButton_Click

        private void returnToLoginButton_Click(object sender, RoutedEventArgs e)
        {
            //close database connection
            cnn.Close();

            //return to login window
            Window loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }//end returnToLoginButton_Click method
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________