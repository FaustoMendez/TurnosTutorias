using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using ServiceContracts;
using ServiceContracts.DTOs;
using ServiceHost.Callbacks;
using ServiceHost.Properties;   
using Data;                      

namespace ServiceHost
{
    public partial class TutorPage : Page
    {
        private readonly IUserService userService;
        private readonly ObservableCollection<TutorDto> tutors;

        public TutorPage()
        {
            InitializeComponent();

            tutors = new ObservableCollection<TutorDto>();
            TutorsGrid.ItemsSource = tutors;

            var factory = new ChannelFactory<IUserService>("NetTcpBinding_IUserService");
            userService = factory.CreateChannel();

            Loaded += OnPageLoaded;
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            LoadTutorsGrid();
        }

        private void LoadTutorsGrid()
        {
            try
            {
                tutors.Clear();
                foreach (var tutor in userService.GetAllTutors())
                    tutors.Add(tutor);
            }
            catch (FaultException fe)
            {
                MessageBox.Show(
                    fe.Message,
                    Properties.Resources.ValidationErrorTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(Properties.Resources.UnexpectedErrorMessage, ex.Message),
                    Properties.Resources.ErrorTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void RegisterTutor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Validators.ValidateTutorId(TxtTutorId.Text, nameof(TxtTutorId));
                Validators.ValidateFirstName(TxtFirstName.Text, nameof(TxtFirstName));
                Validators.ValidateName(TxtPaternalSurname.Text, nameof(TxtPaternalSurname));
                Validators.ValidateName(TxtMaternalSurname.Text, nameof(TxtMaternalSurname));
                Validators.ValidatePhone(TxtPhone.Text, nameof(TxtPhone));
                Validators.ValidateEmail(TxtEmail.Text, nameof(TxtEmail));
                Validators.ValidatePassword(TxtPassword.Password, nameof(TxtPassword));

                if (string.IsNullOrWhiteSpace(TxtPassword.Password))
                    throw new ArgumentException(
                        Properties.Resources.Validators_Password_Required, nameof(TxtPassword));

                var request = new RegisterTutorRequest
                {
                    TutorId = TxtTutorId.Text.Trim(),
                    FirstName = TxtFirstName.Text.Trim(),
                    PaternalSurname = TxtPaternalSurname.Text.Trim(),
                    MaternalSurname = TxtMaternalSurname.Text.Trim(),
                    Phone = TxtPhone.Text.Trim(),
                    Email = TxtEmail.Text.Trim(),
                    Password = TxtPassword.Password
                };

                userService.RegisterTutor(request);

                MessageBox.Show(
                    Properties.Resources.TutorRegisteredSuccessfully,
                    Properties.Resources.OKButtonText,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                LoadTutorsGrid();
                TxtTutorId.Clear();
                TxtFirstName.Clear();
                TxtPaternalSurname.Clear();
                TxtMaternalSurname.Clear();
                TxtPhone.Clear();
                TxtEmail.Clear();
                TxtPassword.Password = string.Empty;
            }
            catch (ArgumentException ex)
            {
                var raw = ex.Message;
                var markerEs = "Nombre del parámetro";
                var markerEn = "Parameter name";
                var idxEs = raw.IndexOf(markerEs, StringComparison.OrdinalIgnoreCase);
                var idxEn = raw.IndexOf(markerEn, StringComparison.OrdinalIgnoreCase);
                var idx = idxEs >= 0 ? idxEs : (idxEn >= 0 ? idxEn : -1);
                var clean = idx >= 0
                    ? raw.Substring(0, idx).TrimEnd('.', ' ', '\r', '\n')
                    : raw.Trim();

                var key = $"Field_{ex.ParamName}";
                var friendly = Properties.Resources.ResourceManager
                                   .GetString(key)
                               ?? ex.ParamName;

                MessageBox.Show(
                    $"{clean}. {friendly}",
                    Properties.Resources.ValidationErrorTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (FaultException fe)
            {
                MessageBox.Show(
                    fe.Message,
                    Properties.Resources.ValidationErrorTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(Properties.Resources.UnexpectedErrorMessage, ex.Message),
                    Properties.Resources.ErrorTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void RefreshTutors_Click(object sender, RoutedEventArgs e)
            => LoadTutorsGrid();

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var pwPlaceholder = (PasswordBox)sender;
            var placeholder = pwPlaceholder
                .Template
                .FindName("PART_Placeholder", pwPlaceholder) as TextBlock;
            if (placeholder == null) return;

            placeholder.Visibility =
                string.IsNullOrEmpty(pwPlaceholder.Password)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
    }
}
