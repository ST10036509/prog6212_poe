using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;


namespace prog6212_poe
{
    /// <summary>
    /// Interaction logic for PlannerSemestersWindow.xaml
    /// </summary>
    public partial class PlannerSemestersWindow : Window
    {
        //carry over variables:
        readonly private SqlConnection cnn;
        int userID;
        List<MySemesterItem> mySemesterItems = new List<MySemesterItem>();

        //----------------------------------------------------------------------------------------------Constructors

        //Constructor
        public PlannerSemestersWindow()
        {
            InitializeComponent();

            //fetch the userID of logged in user
            userID = (int)App.Current.Properties["UserID"];

            //establish database connection string
            var connectionString = App.Current.Properties["ConnectionString"] as String;

            //create a connection usning connection string
            cnn = new SqlConnection(connectionString);
            //open the connection
            cnn.Open();

            DisplaySemesters();
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
        }//end Disable The Window Close Button

        //----------------------------------------------------------------------------------------------DisplaySemesters

        //display the semesters in the list view
        public async void DisplaySemesters()        
        {
            //create an new semester DataModel for the selected semester
            var semesterData = await Task.Run(() => GetSemesterData());

            //populate ListView with semester names
            semestersListView.ItemsSource = semesterData;
        }//end DisplaySemsters method 

        public async Task<List<MySemesterItem>> GetSemesterData()
        {
            //create a sql query to get the semester data
            string query = "SELECT SemesterID, SemesterName FROM Semesters WHERE UserID = @UserID";
            SqlCommand command = new SqlCommand(query, cnn);
            command.Parameters.AddWithValue("@UserID", userID);

            //use a sqlreader to capture the semester data and place it into a datamodel
            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    //semesterNames.Add(reader["SemesterName"].ToString());
                    mySemesterItems.Add(new MySemesterItem
                    {
                        Name = reader["SemesterName"].ToString(),
                        SemesterID = (int)reader["SemesterID"]
                    });
                }
            }

            //return the datamodel
            return mySemesterItems;
        }//end GetSemesterNames method

        //----------------------------------------------------------------------------------------------MySemesterItem Class

        //class to store semester name items for DataLink to window
        public class MySemesterItem
        {
            public string Name { get; set; }
            public int SemesterID { get; set; }
        }//end MySemesterItem class

        //----------------------------------------------------------------------------------------------selectSemesterButton_Click

        //select a semester && open the modules selection window
        private void selectSemesterButton_Click(object sender, RoutedEventArgs e)
        {
            //check if no semester has been selected
            if (!(semestersListView.SelectedIndex >= 0))
            {
                //error message if no semester is selected
                messageTextBlock.Visibility = Visibility.Visible;
                return;
            }

            //fetch the index of the selected semester
            var index = semestersListView.SelectedIndex;

            //close connection
            cnn.Close();

            //open module selection window
            Window viewModulesWindow = new PlannerModulesWindow(mySemesterItems[index].SemesterID);
            viewModulesWindow.Show();
            this.Close();
        }//end selectSemesterButton_Click method

        //----------------------------------------------------------------------------------------------semestersListView_SelectionChanged

        private void semestersListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //hide error message
            messageTextBlock.Visibility = Visibility.Hidden;
        }//end semestersListView_SelectionChanged method

        //----------------------------------------------------------------------------------------------returnToMainMenuButton_Click

        //return to Main Menu page
        private void returnToMainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            //close connection
            cnn.Close();

            //open main window
            Window mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }//end returnToMainMenuButton_Click method
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________