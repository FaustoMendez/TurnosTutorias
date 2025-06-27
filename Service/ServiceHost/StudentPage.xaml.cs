using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using ServiceContracts;
using ServiceContracts.DTOs;
using ServiceHost.Callbacks;
using ServiceHost.Properties;
using Data;
using ServiceContracts.DTOs.ServiceContracts.DTOs;

namespace ServiceHost
{
    public partial class StudentPage : Page
    {
        private readonly ITutoringService shiftService;
        private readonly IUserService userService;
        private readonly ObservableCollection<AppointmentDto> pendingAppointments;
        private bool isInitialized;

        public StudentPage()
        {
            InitializeComponent();

            pendingAppointments = new ObservableCollection<AppointmentDto>();
            QueueGrid.ItemsSource = pendingAppointments;

            var callbackHandler = new NotificationCallbackHandler();
            callbackHandler.NewPendingAppointment += dto =>
                Dispatcher.Invoke(() => pendingAppointments.Insert(0, dto));

            var context = new InstanceContext(callbackHandler);
            var duplexFactory = new DuplexChannelFactory<ITutoringService>(
                context, "NetTcpBinding_ITutoringService");
            shiftService = duplexFactory.CreateChannel();

            var userFactory = new ChannelFactory<IUserService>(
                "NetTcpBinding_IUserService");
            userService = userFactory.CreateChannel();

            Loaded += OnPageLoaded;
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            if (isInitialized) return;
            isInitialized = true;

            LoadStudents();
            LoadTutors();
            LoadQueue();
            LoadCareers();
        }

        private void LoadQueue()
        {
            pendingAppointments.Clear();
            foreach (var appointment in shiftService.GetPendingAppointments())
                pendingAppointments.Add(appointment);
        }

        private void LoadTutors()
        {
            try
            {
                CbTutors.ItemsSource = userService.GetAvailableTutors();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{Properties.Resources.ErrorLoadingTutors}\n{ex.Message}",
                    Properties.Resources.ErrorTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void LoadCareers()
        {
            try
            {
                CbCareers.ItemsSource = userService.GetAllCareers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{Properties.Resources.ErrorLoadingCareers}\n{ex.Message}",
                    Properties.Resources.ErrorTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void LoadStudents()
        {
            var users = userService.GetAllUsers();
            var tutors = userService
                .GetAvailableTutors()
                .ToDictionary(t => t.TutorId, t => t.FullName);
            var careers = userService
                .GetAllCareers()
                .ToDictionary(c => c.CareerId, c => c.CareerName);

            var items = users.Select(u => new
            {
                u.UserId,
                Name = $"{u.FirstName} {u.PaternalSurname} {u.MaternalSurname}",
                u.Email,
                Career = careers[u.CareerId],
                Tutor = tutors[u.TutorId]
            });

            UsersGrid.ItemsSource = items.ToList();
        }

        private void RegisterStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Validators.ValidateMatricula(TxtStudentId.Text, nameof(TxtStudentId));
                Validators.ValidateFirstName(TxtFirstName.Text, nameof(TxtFirstName));
                Validators.ValidateName(TxtPaternalSurname.Text, nameof(TxtPaternalSurname));
                Validators.ValidateName(TxtMaternalSurname.Text, nameof(TxtMaternalSurname));
                Validators.ValidateEmail(TxtEmail.Text, nameof(TxtEmail));
                Validators.ValidatePassword(TxtPassword.Password, nameof(TxtPassword));

                if (CbCareers.SelectedIndex == -1 || CbTutors.SelectedIndex == -1)
                {
                    MessageBox.Show(
                        Properties.Resources.MustSelectTutorCareer,
                        Properties.Resources.ValidationErrorTitle,
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                var request = new RegisterStudentRequest
                {
                    StudentId = TxtStudentId.Text.Trim(),
                    FirstName = TxtFirstName.Text.Trim(),
                    PaternalSurname = TxtPaternalSurname.Text.Trim(),
                    MaternalSurname = TxtMaternalSurname.Text.Trim(),
                    Email = TxtEmail.Text.Trim(),
                    Password = TxtPassword.Password,
                    CareerId = (int)CbCareers.SelectedValue,
                    TutorId = (string)CbTutors.SelectedValue
                };

                userService.RegisterStudent(request);
                MessageBox.Show(
                    Properties.Resources.StudentRegisteredSuccessfully,
                    Properties.Resources.OKButtonText,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                LoadStudents();
                TxtStudentId.Clear();
                TxtFirstName.Clear();
                TxtPaternalSurname.Clear();
                TxtMaternalSurname.Clear();
                TxtEmail.Clear();
                TxtPassword.Password = string.Empty;
                CbCareers.SelectedIndex = -1;
                CbTutors.SelectedIndex = -1;
            }
            catch (ArgumentException ex)
            {
                var raw = ex.Message;

                var markerEs = "Nombre del parámetro";
                var markerEn = "Parameter name";
                int idxEs = raw.IndexOf(markerEs, StringComparison.OrdinalIgnoreCase);
                int idxEn = raw.IndexOf(markerEn, StringComparison.OrdinalIgnoreCase);
                int idx = idxEs >= 0
                           ? idxEs
                           : (idxEn >= 0 ? idxEn : -1);

                var clean = idx >= 0
                    ? raw.Substring(0, idx).TrimEnd(new[] { ' ', ' ', '\r', '\n' })
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
            catch (FaultException<DuplicateStudentFault> dsf)
            {
                MessageBox.Show(dsf.Detail.Message, 
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

        private void RefreshTutors_Click(object sender, RoutedEventArgs e) => LoadTutors();
        private void RefreshQueue_Click(object sender, RoutedEventArgs e) => LoadQueue();

        private void MarkAttended_Click(object sender, RoutedEventArgs e)
        {
            if (QueueGrid.SelectedItem is AppointmentDto dto)
            {
                try
                {
                    shiftService.FinishSession(dto.AppointmentId);
                    MessageBox.Show(
                        Properties.Resources.ShiftMarkedAttended,
                        Properties.Resources.OKButtonText,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    LoadQueue();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        string.Format(Properties.Resources.ErrorMarkingAttended, ex.Message),
                        Properties.Resources.ErrorTitle,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void MarkNoShow_Click(object sender, RoutedEventArgs e)
        {
            if (QueueGrid.SelectedItem is AppointmentDto dto)
            {
                if (string.IsNullOrWhiteSpace(TxtReason.Text))
                {
                    MessageBox.Show(
                        Properties.Resources.ReasonRequired,
                        Properties.Resources.ValidationErrorTitle,
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }
                try
                {
                    shiftService.NoShowAppointment(dto.AppointmentId, TxtReason.Text.Trim());
                    MessageBox.Show(
                        Properties.Resources.ShiftMarkedNoShow,
                        Properties.Resources.OKButtonText,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    TxtReason.Text = string.Empty;
                    LoadQueue();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        string.Format(Properties.Resources.ErrorMarkingNoShow, ex.Message),
                        Properties.Resources.ErrorTitle,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void CancelShift_Click(object sender, RoutedEventArgs e)
        {
            if (QueueGrid.SelectedItem is AppointmentDto dto)
            {
                if (string.IsNullOrWhiteSpace(TxtReason.Text))
                {
                    MessageBox.Show(
                        Properties.Resources.ReasonRequired,
                        Properties.Resources.ValidationErrorTitle,
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }
                try
                {
                    shiftService.CancelAppointment(dto.AppointmentId, TxtReason.Text.Trim());
                    MessageBox.Show(
                        Properties.Resources.ShiftCancelled,
                        Properties.Resources.OKButtonText,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    TxtReason.Text = string.Empty;
                    LoadQueue();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        string.Format(Properties.Resources.ErrorCancellingShift, ex.Message),
                        Properties.Resources.ErrorTitle,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;
            var placeholder = passwordBox.Template.FindName("PART_Placeholder", passwordBox) as TextBlock;
            if (placeholder == null) return;

            placeholder.Visibility = string.IsNullOrEmpty(passwordBox.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }
}




