using System.Windows;
using System.Windows.Controls;

namespace ServiceHost
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new StudentPage());
        }

        private void OnManageStudents_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new StudentPage());
        }

        private void OnManageTutors_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TutorPage());
        }
    }
}

