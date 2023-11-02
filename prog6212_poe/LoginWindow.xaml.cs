using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using HoursForYourLib;

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
            //App.Current.Properties["ConnectionString"] = @"Data Source=LAPTOP-OMEN;Initial Catalog=HoursForYouDB;Integrated Security=True";
            App.Current.Properties["ConnectionString"] = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename=|DataDirectory|\HoursForYouDB.mdf; Integrated Security = True; Connect Timeout = 30";

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
            string password = passwordTextBox.Password;
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
                PasswordHandler handler = new PasswordHandler();
                var validPassword = await Task.Run(() => handler.ValidatePassword(dbPassword, password));

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
                    App.Current.Properties["UserID"] = userID;

                    //close database connection
                    cnn.Close();

                    //open main window window
                    Window mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
            }
        }//end loginButton_Click method

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