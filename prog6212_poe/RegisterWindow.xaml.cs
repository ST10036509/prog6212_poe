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

        SqlConnection cnn;
        bool usernameExists;

        public RegisterWindow()
        {
            InitializeComponent();

            string connectionString = @"Data Source=LAPTOP-OMEN;Initial Catalog=HoursForYouDB;Integrated Security=True";

            cnn = new SqlConnection(connectionString);
            cnn.Open();
        }

        public async Task<bool> CheckIfUsernameExists(string username)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username;";

            SqlCommand command = new SqlCommand(query, cnn);
            command.Parameters.AddWithValue("@Username", username);

            //check if the username exists
            int count = Convert.ToInt32(await command.ExecuteScalarAsync());
            return count > 0;//return true if the username exists and false if it doesnt
        }

        private async void usernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string username = usernameTextBox.Text;

            usernameExists = await Task.Run(() => CheckIfUsernameExists(username));

            if (usernameExists)
            {
                messageTextBlock.Text = "USERNAME Is Unavaliable!";
                messageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                messageTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                messageTextBlock.Visibility = Visibility.Hidden;
            }
        }

        private void confirmationPasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (confirmationPasswordTextBox.Text != passwordTextBox.Text)
            {
                messageTextBlock.Text = "Your PASSWORDS Dont Match!";
                messageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                messageTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                messageTextBlock.Visibility = Visibility.Hidden;
            }
        }

        private async void registerButton_Click(object sender, RoutedEventArgs e)
        {

            if ( (usernameExists) )
            {
                messageTextBlock.Text = "USERNAME Is Unavaliable!";
                messageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                messageTextBlock.Visibility = Visibility.Visible;
            }
            else if ( (confirmationPasswordTextBox.Text != passwordTextBox.Text) )
            {
                messageTextBlock.Text = "Your PASSWORDS Dont Match!";
                messageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                messageTextBlock.Visibility = Visibility.Visible;
            }
            else if ( (usernameTextBox.Text == "") )
            {
                messageTextBlock.Text = "USERNAME Cannot Be Empty!";
                messageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                messageTextBlock.Visibility = Visibility.Visible;
            }
            else if ( (passwordTextBox.Text == "") || (confirmationPasswordTextBox.Text == ""))
            {
                messageTextBlock.Text = "PASSWORD Cannot Be Empty!";
                messageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                messageTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                string username = usernameTextBox.Text;
                string password = passwordTextBox.Text;

                var hashedPassword = await Task.Run(() => EncryptPassword(password));

                await Task.Run(() => AddUser(username, hashedPassword));

                //open main window window
                Window mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }

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
        }

        public async Task AddUser(string username, string password)
        {
            string query = "INSERT INTO Users (Username, [Password]) VALUES (@Username, @Password);";
            SqlCommand command = new SqlCommand(query, cnn);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);

            await command.ExecuteNonQueryAsync();
        }

        private void returnToLoginButton_Click(object sender, RoutedEventArgs e)
        {
            cnn.Close();
            //retuen to login window
            Window loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
