using HoursForYourLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
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
        //declare GLOBAL variables:
        readonly private SqlConnection cnn;

        //----------------------------------------------------------------------------------------------constructor

        public LoginWindow()
        {
            InitializeComponent();

            //store the connection string in the application properties
            App.Current.Properties["ConnectionString"] = @"Data Source=LAPTOP-OMEN;Initial Catalog=HoursForYouDB;Integrated Security=True";

            //establish database connection string
            var connectionString = App.Current.Properties["ConnectionString"] as String;

            //create a connection usning connection string
            cnn = new SqlConnection(connectionString);
            //open the connection
            cnn.Open();
        }//end constructor method

        //----------------------------------------------------------------------------------------------loginButton_Click

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            //fetch entered data
            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;
            //check if a matching user exists in the database and return the password if it does
            (int userID, string dbPassword) = await Task.Run(() => GetPasswordForUsername(username));

            //if no user exists
            if (userID == -1)
            {
                //display error messsage
                messageTextBlock.Text = "Invalid USERNAME!";
                messageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                messageTextBlock.Visibility = Visibility.Visible;
            }
            //if a user does exists
            else
            {
                //validate entered password
                var validPassword = await Task.Run(() => ValidatePassword(dbPassword, password));

                //if the password isnt valid
                if (!validPassword)
                {
                    //display error message
                    messageTextBlock.Text = "Incorrect PASSWORD!";
                    messageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                    messageTextBlock.Visibility = Visibility.Visible;
                }
                //if the password is valid
                else
                {
                    //close database connection
                    cnn.Close();

                    //open main window window
                    Window mainWindow = new MainWindow(userID);
                    mainWindow.Show();
                    this.Close();
                }
            }
        }//end loginButton_Click method

        //----------------------------------------------------------------------------------------------ValidatePassword

        public async Task<bool> ValidatePassword(string dbPassword, string userPassword)
        {
            //dewcode the password string to bytes
            byte[] dbHashBytes = Convert.FromBase64String(dbPassword);

            //extract the salt from the password hash
            byte[] salt = new byte[20];
            Array.Copy(dbHashBytes, 0, salt, 0, 20);

            //use the ectracted salt to encrypt the given password into a has byte sequence
            var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(userPassword, salt, 10000);
            byte[] enteredHash = pbkdf2.GetBytes(20);

            //comapre the two hash sequences byte by byte
            for (int i = 0; i < 20; i++)
            {
                //if any byte is different, the passwords do not match
                if (dbHashBytes[i + 20] != enteredHash[i])
                {
                    //fault out
                    return false;
                }
            }
            //if all bytes match, the passwords are the same
            return true;
        }//end ValidatePassword method

        //----------------------------------------------------------------------------------------------GetPasswordForUsername

        public async Task<(int, string)> GetPasswordForUsername(string username)
        {
            //create a SQL query to fetch the password of a matching username
            string query = "SELECT UserID, [Password] FROM Users WHERE Username = @Username;";
            //create a new command
            SqlCommand command = new SqlCommand(query, cnn);
            //assign variables to the sql query parameters
            command.Parameters.AddWithValue("@Username", username);

            //execute the query
            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                //if a result is returned
                if (await reader.ReadAsync())
                {
                    //fetch the userid and password for return
                    int userID = reader.GetInt32(0);
                    string password = reader.GetString(1);
                    return (userID, password);
                }
                //if no results are found
                else
                {
                    //send back null value
                    return (-1, null);
                }
            }
        }//end GetPasswordForUsername method

        //----------------------------------------------------------------------------------------------registerButton_Click

        private void registerButton_Click(object sender, MouseButtonEventArgs e)
        {
            //close database connection
            cnn.Close();

            //open register window
            Window registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }//end registerButton_Click method

        //----------------------------------------------------------------------------------------------exitProgramButton_Click

        private void exitProgramButton_Click(object sender, RoutedEventArgs e)
        {
            //close database connection
            cnn.Close();

            //exit application
            Environment.Exit(0);
        }//end exitProgramButton_Click method
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________