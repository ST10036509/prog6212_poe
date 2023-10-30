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
using System.Data.SqlClient;
using System.IO.Packaging;
using System.Security.Cryptography;

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
                var hashedPassword = await Task.Run(() => EncryptPassword(password));

                //add user to database
                var userID = await Task.Run(() => AddUser(username, hashedPassword));

                //close database connection
                cnn.Close();

                //open main window 
                Window mainWindow = new MainWindow(userID);
                mainWindow.Show();
                this.Close();
            }
        }//end registerButton_Click method

        //----------------------------------------------------------------------------------------------EncryptPassword

        //using PBKDF2 to encrypt the password with SALTING
        public async Task<string> EncryptPassword(string password)
        {
            //create the salt (20 bytes long) for password encryption and decryption using cryptographic PRNG
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[20]);

            //create the Rfc2898DeriveBytes
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            //get the hash value (20 bytes long)
            byte[] hash = pbkdf2.GetBytes(20);

            //combine the salt with the hash starting with the salt
            byte[] saltedHashBytes = new byte[40];
            Array.Copy(salt, 0, saltedHashBytes, 0, 20);//past from index 0 -- 20
            Array.Copy(hash, 0, saltedHashBytes, 20, 20);//paste from index 20 -- 40

            //return a string of the salt+hash in base64 format for storage
            return Convert.ToBase64String(saltedHashBytes);
        }//end EncryptPassword method

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