using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TutoriasClient
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            this.Loaded += LoginView_Loaded;
        }

        private void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            var current = Thread.CurrentThread.CurrentUICulture.Name;
            foreach (ComboBoxItem it in LanguageSelector.Items)
                if ((string)it.Tag == current)
                {
                    LanguageSelector.SelectedItem = it;
                    break;
                }

            LanguageSelector.SelectionChanged += LanguageSelector_SelectionChanged;
            Loaded -= LoginView_Loaded;
        }

        private void LanguageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(LanguageSelector.SelectedItem is ComboBoxItem item)) return;
            var code = item.Tag as string;
            if (string.IsNullOrEmpty(code)) return;

            var ci = new CultureInfo(code);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            var win = new LoginView();
            Application.Current.MainWindow = win;
            win.Show();
            this.Close();
        }
    }
}
