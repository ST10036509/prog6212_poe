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
    /// Interaction logic for AddModuleWindow.xaml
    /// </summary>
    public partial class AddModuleWindow : Window
    {
        public AddModuleWindow()
        {
            InitializeComponent();
        }//end constructor

        //add module and return to create semester page
        private void AddModuleButton_Click(object sender, RoutedEventArgs e)
        {
            Window createSemesterWin = new SemesterCreationWindow();
            createSemesterWin.Show();
            this.Close();
        }//end addModuleButton_Click method

        //return to create semester page
        private void ReturnToCreateSemesterButton_Click(object sender, RoutedEventArgs e)
        {
            Window createSemesterWin = new SemesterCreationWindow();
            createSemesterWin.Show();
            this.Close();
        }//end returnToCreateSemesterButton_Click method
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________