using System.Windows.Controls;
using ServiceContracts;
using ServiceContracts.DTOs;
using System.ServiceModel;
using System.Windows;

namespace ServiceHost
{
    public partial class StudentPage : Page
    {
        private readonly IUserService _svc;
        public StudentPage()
        {
            InitializeComponent();
            var factory = new ChannelFactory<IUserService>("NetTcpBinding_IUserService");
            _svc = factory.CreateChannel();
            Loaded += (s, e) => {
                RefreshTutors();
                RefreshUsers();
            };
        }

        private void RefreshTutors() { /*…*/ }
        private void RefreshUsers() { /*…*/ }
        private void RegisterStudent_Click(object s, RoutedEventArgs e) { /*…*/ }
        private void RefreshTutors_Click(object s, RoutedEventArgs e) { /*…*/ }
    }
}

