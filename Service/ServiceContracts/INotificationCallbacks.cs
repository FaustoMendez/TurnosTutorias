using ServiceContracts.DTOs;
using ServiceContracts.DTOs.ServiceContracts.DTOs;
using System.ServiceModel;

namespace ServiceContracts
{
    public interface INotificationCallback
    {
        [OperationContract(IsOneWay = true)]
        void NotifyUpcomingAppointment(AppointmentDto appointment);
    }
}
