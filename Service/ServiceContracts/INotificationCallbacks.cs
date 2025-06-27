using ServiceContracts.DTOs;
using ServiceContracts.DTOs.ServiceContracts.DTOs;
using System.ServiceModel;

namespace ServiceContracts
{
    public interface INotificationCallbacks
    {
        [OperationContract(IsOneWay = true)]
        void NotifyUpcomingAppointment(AppointmentDto appointment);
    }
}
