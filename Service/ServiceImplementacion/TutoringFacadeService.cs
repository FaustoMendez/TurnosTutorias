using System;
using System.Collections.Generic;
using System.ServiceModel;
using ServiceContracts;
using ServiceContracts.DTOs;
using ServiceImplementacion.Business;
using Data;
using ServiceContracts.DTOs.ServiceContracts.DTOs;

namespace ServiceImplementacion.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class TutoringFacadeService : IUserService, ITutoringService
    {
        private readonly UserManager _userManager;
        private readonly AppointmentManager _appointmentManager;
        private readonly NotificationManager _notificationManager;


        public TutoringFacadeService()
        {
            var ctx = new TurnosTutoriasEntities();
            _userManager = new UserManager(ctx);
            _appointmentManager = new AppointmentManager(ctx);
        }

        public void RegisterTutor(RegisterTutorRequest request) => _userManager.RegisterTutor(request);

        public void RegisterStudent(RegisterStudentRequest request) => _userManager.RegisterStudent(request);

        public List<UserDto> GetAllUsers() => _userManager.GetAllUsers();

        public AuthenticatedUserDto LoginUser(LoginRequest request) => _userManager.LoginUser(request);

        public int CreateAppointment(CreateAppointmentRequest request) => _appointmentManager.CreateAppointment(request.StudentId, request.TutorId, request.SessionId).TurnoId;

        public List<AppointmentDto> GetPendingAppointments() => _appointmentManager.GetPendingAppointments();

        public void CancelAppointment(int appointmentId, string reason) => _appointmentManager.CancelAppointment(appointmentId, reason);

        public void NoShowAppointment(int appointmentId, string reason) => _appointmentManager.NoShowAppointment(appointmentId, reason);

        public void RegisterSession(int appointmentId, DateTime date, TimeSpan startTime, TimeSpan endTime) => _appointmentManager.RegisterSession(appointmentId, date, startTime, endTime);

        public void EvaluateAppointment(FinalEvaluationRequest request) => _appointmentManager.EvaluateAppointment(request);

        public void FinishSession(int appointmentId) => _appointmentManager.FinishSession(appointmentId);

        public List<TutorDto> GetAvailableTutors() => _userManager.GetAvailableTutors();

        public void SubscribeForNotifications(string studentId) => _notificationManager.Subscribe(studentId, OperationContext.Current.GetCallbackChannel<INotificationCallback>());

        public void UnsubscribeFromNotifications(string studentId) => _notificationManager.Unsubscribe(studentId);
    }
}
