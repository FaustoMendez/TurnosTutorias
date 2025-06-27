using System;
using System.Windows;
using ServiceContracts;
using ServiceContracts.DTOs;
using ServiceContracts.DTOs.ServiceContracts.DTOs;
using ServiceHost.Properties;

namespace ServiceHost.Callbacks
{
    public class NotificationCallbackHandler : INotificationCallbacks
    {
        public event Action<AppointmentDto> NewPendingAppointment;

        public void NotifyUpcomingAppointment(AppointmentDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            try
            {
                string message = string.Format(
                    Resources.NotifyUpcomingMessageFormat,
                    dto.AppointmentId,
                    dto.SessionDate.ToString("G", System.Globalization.CultureInfo.CurrentCulture)
                );

                MessageBox.Show(
                    message,
                    Resources.NotifyUpcomingTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format(
                    Resources.NotifyErrorMessageFormat,
                    ex.Message
                );
                MessageBox.Show(
                    errorMessage,
                    Resources.NotifyErrorTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        public void NotifyNewPending(AppointmentDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            try
            {
                NewPendingAppointment?.Invoke(dto);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format(
                    Resources.NotifyErrorMessageFormat,
                    ex.Message
                );
                MessageBox.Show(
                    errorMessage,
                    Resources.NotifyErrorTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}
