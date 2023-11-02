using System;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace prog6212_poe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //carry over variables:
        //private List<Semester> semesters = new List<Semester>();
        int userID;
        readonly private SqlConnection cnn;

        //----------------------------------------------------------------------------------------------Constuctors

        //OVERLOADED DB Constructor (LOGIN/REGISTER >> MAIN WINDOW)
        public MainWindow()
        {
            InitializeComponent();

            userID = (int)App.Current.Properties["UserID"];

            //establish database connection string
            var connectionString = App.Current.Properties["ConnectionString"] as String;

            //create a connection usning connection string
            cnn = new SqlConnection(connectionString);
            //open the connection
            cnn.Open();
        }//end constructor

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
        //End Disable The Window Close Button

        //----------------------------------------------------------------------------------------------AddSemesterButton_Click

        //open semester page
        private void AddSemesterButton_Click(object sender, RoutedEventArgs e)
        {
            cnn.Close();

            //open semester creation window
            Window createSemesterWindow = new SemesterCreationWindow();
            createSemesterWindow.Show();
            this.Close();
        }//end AddSemesterButton_Click method

        //----------------------------------------------------------------------------------------------PlannerBookButton_Click

        //open planner page
        private async void PlannerBookButton_Click(object sender, RoutedEventArgs e)
        {
            var semesterExists = await Task.Run(() => VerifySemestersDatabase());

            //check if there are any semesters
            if (semesterExists)
            {
                //close connection
                cnn.Close();

                //open planner page
                Window viewSemestersWindow = new PlannerSemestersWindow();
                viewSemestersWindow.Show();
                this.Close();
            }
            else
            {
                //display error message
                messageTextBlock.Text = "Please make sure you create at least ONE semester before proceeding!";
                messageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                messageTextBlock.Visibility = Visibility.Visible;
            }
        }//end PlannerBookButton_Click method

        //----------------------------------------------------------------------------------------------VerifySemestersDatabase

        public async Task<bool> VerifySemestersDatabase()
        {
            string query = "SELECT COUNT(*) FROM Semesters WHERE UserID = @UserID;";
            SqlCommand command = new SqlCommand(query, cnn);
            command.Parameters.AddWithValue("@UserID", userID);

            int semesterCount = (int)await command.ExecuteScalarAsync();
            return semesterCount > 0;
        }//end VerifySemestersDatabase method

        //----------------------------------------------------------------------------------------------ExitProgramButton_Click

        //close program
        private void ExitProgramButton_Click(object sender, RoutedEventArgs e)
        {
            cnn.Close();

            Environment.Exit(0);
        }//end exitProgramButton_Click method
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________