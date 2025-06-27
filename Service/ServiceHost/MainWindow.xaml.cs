using System;
using System.Globalization;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ServiceHost
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            MainFrame.Navigate(new StudentPage());
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var current = Thread.CurrentThread.CurrentUICulture.Name;
            foreach (ComboBoxItem it in LanguageSelector.Items)
                if ((string)it.Tag == current)
                {
                    LanguageSelector.SelectedItem = it;
                    break;
                }

            LanguageSelector.SelectionChanged += LanguageSelector_SelectionChanged;
            Loaded -= MainWindow_Loaded;
        }

        private void OnManageStudents_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new StudentPage());

        private void OnManageTutors_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new TutorPage());

        private void OnReports_Click(object sender, RoutedEventArgs e)
            => MainFrame.Content = new ReportsPage();

        private void BtnStopServer_Click(object sender, RoutedEventArgs e)
        {
            var app = (App)Application.Current;
            try
            {
                app.StopWcfHost();
                MessageBox.Show("✅ Service stopped", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"⚠ Error stopping service:\n{ex.Message}",
                                "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                Application.Current.Shutdown();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((App)Application.Current).StopWcfHost();
        }

        private void LanguageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(LanguageSelector.SelectedItem is ComboBoxItem item)) return;
            var code = item.Tag as string;
            if (string.IsNullOrEmpty(code)) return;

            var ci = new CultureInfo(code);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            var win = new MainWindow();
            Application.Current.MainWindow = win;
            win.Show();
            this.Close();
        }
    }
}
