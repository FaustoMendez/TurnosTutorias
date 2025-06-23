using System;
using System.Collections.Generic;
using System.ServiceModel;
using ServiceContracts.DTOs;
using ServiceContracts.DTOs.ServiceContracts.DTOs;

namespace ServiceContracts
{
    [ServiceContract(CallbackContract = typeof(INotificationCallback))]
    public interface ITutoringService
    {
        [OperationContract]
        int CreateAppointment(CreateAppointmentRequest request);

        [OperationContract]
        List<AppointmentDto> GetPendingAppointments();

        [OperationContract]
        void CancelAppointment(int appointmentId, string reason);

        [OperationContract]
        void NoShowAppointment(int appointmentId, string reason);

        [OperationContract]
        void RegisterSession(int appointmentId, DateTime date, TimeSpan startTime, TimeSpan endTime);

        [OperationContract]
        void EvaluateAppointment(FinalEvaluationRequest request);

        [OperationContract(IsOneWay = true)]
        void SubscribeForNotifications(string studentId);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeFromNotifications(string studentId);

        [OperationContract]
        void FinishSession(int appointmentId);
    }
}
