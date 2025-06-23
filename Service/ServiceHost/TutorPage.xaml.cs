using System.Windows;
using System.Windows.Controls;
using ServiceImplementacion.Business;
using Data;
using ServiceContracts.DTOs;

namespace ServiceHost
{
    public partial class TutorPage : Page
    {
        private readonly UserManager _userMgr;

        public TutorPage()
        {
            InitializeComponent();
            _userMgr = new UserManager(new TurnosTutoriasEntities());
            RefreshTutors();
        }

        private void RefreshTutors_Click(object sender, RoutedEventArgs e)
        {
            var tutors = _userMgr.GetAvailableTutors();
            TutorsGrid.ItemsSource = tutors;
        }

        private void RegisterTutor_Click(object sender, RoutedEventArgs e)
        {
            var req = new RegisterTutorRequest
            {
                TutorId = TxtTutorId.Text,
                FirstName = TxtFirstName.Text,
                PaternalSurname = TxtPaternalSurname.Text,
                MaternalSurname = TxtMaternalSurname.Text,
                Phone = TxtPhone.Text,
                Email = TxtEmail.Text,
                Password = TxtPassword.Password
            };

            _userMgr.RegisterTutor(req);
            RefreshTutors();
            MessageBox.Show("Tutor registered successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RefreshTutors() { /*…*/ }

    }
}
