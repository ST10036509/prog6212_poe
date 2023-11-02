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
    /// Interaction logic for PlannerModulesWindow.xaml
    /// </summary>
    public partial class PlannerModulesWindow : Window
    {
        //carry over variables:
        int semesterID;
        readonly private SqlConnection cnn;
        List<MyModuleItem> myModuleItems = new List<MyModuleItem>();

        //----------------------------------------------------------------------------------------------Constuctors

        //Constructor
        public PlannerModulesWindow()
        {
            InitializeComponent();
        }//end constructor

        //OVERLOADED DB Constructor
        public PlannerModulesWindow(int semesterID)
        {
            InitializeComponent();

            this.semesterID = semesterID;

            //establish database connection string
            var connectionString = App.Current.Properties["ConnectionString"] as String;

            //create a connection usning connection string
            cnn = new SqlConnection(connectionString);
            //open the connection
            cnn.Open();

            //display modules in list view
            DisplayModules();
        }//end OVERLOADED DB Constructor

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
        //end Disable The Window Close Button

        //----------------------------------------------------------------------------------------------DisplayModules

        //display the modules in the list view
        public async void DisplayModules()
        {
            //get the module data
            var moduleData = await Task.Run(() => GetModuleData());
            //display the module data in the list view
            modulesListView.ItemsSource = moduleData;
        }//end DisplayModules method

        public async Task<List<MyModuleItem>> GetModuleData()
        {
            //create sql query to get the module data
            string query = "SELECT ModuleID, ModuleName, SelfStudyHours FROM Modules WHERE SemesterID = @SemesterID";
            //create sql command and set paramter values
            SqlCommand command = new SqlCommand(query, cnn);
            command.Parameters.AddWithValue("@SemesterID", semesterID);

            //use a sqlreader to capture the module data and place it into a datamodel
            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    myModuleItems.Add(new MyModuleItem
                    {
                        ModuleID = (int)reader["ModuleID"],
                        Name = reader["ModuleName"].ToString(),
                        Hours = reader["SelfStudyHours"].ToString()
                    });
                }
            }
            //return datamodel
            return myModuleItems;
        }//end GetSemesterNames method

        //----------------------------------------------------------------------------------------------MyModuleItem class

        //class to hold module data
        public class MyModuleItem
        {
            public int ModuleID { get; set; }
            public string Name { get; set; }
            public string Hours { get; set; }
        }//end MyModuleItem class

        //----------------------------------------------------------------------------------------------selectModuleButton_Click

        //select a module && go to the module details window
        private void selectModuleButton_Click(object sender, RoutedEventArgs e)
        {
            //check if no module has been selected
            if (!(modulesListView.SelectedIndex >= 0))
            {
                //error message if no module is selected
                messageTextBlock.Visibility = Visibility.Visible;
                return;
            }

            //fetch the index of the selected module
            var index = modulesListView.SelectedIndex;

            //close connection
            cnn.Close();

            //open module view window
            Window moduleDetailsWindow = new PlannerModuleViewWindow(myModuleItems[index].ModuleID, semesterID);
            moduleDetailsWindow.Show();
            this.Close();
        }//end selectModuleButton_Click method

        //----------------------------------------------------------------------------------------------modulesListView_SelectionChanged

        private void modulesListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //hide error message
            messageTextBlock.Visibility = Visibility.Hidden;
        }//end modulesListView_SelectionChanged method

        //----------------------------------------------------------------------------------------------returnToSemesterViewButton_Click

        //return to the semester view window
        private void returnToSemesterViewButton_Click(object sender, RoutedEventArgs e)
        {
            //close connection
            cnn.Close();

            //open planner semester window
            Window viewSemestersWindow = new PlannerSemestersWindow();
            viewSemestersWindow.Show();
            this.Close();
        }//end returnToSemesterViewButton_Click method
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________