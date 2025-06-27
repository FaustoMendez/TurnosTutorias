using System;
using System.Collections.Generic;
using System.ServiceModel;
using ServiceContracts;
using ServiceContracts.DTOs;
using ServiceImplementacion.Business;
using Data;
using ServiceContracts.DTOs.ServiceContracts.DTOs;
using System.Diagnostics;

namespace ServiceImplementacion
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class TutoringFacadeService : IUserService, ITutoringService
    {
        private readonly UserManager userManager;
        private readonly AppointmentManager appointmentManager;
        private readonly NotificationManager notificationManager;


        public TutoringFacadeService()
        {
            var ctx = new TurnosTutoriasEntities();
            userManager = new UserManager(ctx);
            appointmentManager = new AppointmentManager(ctx);
        }

        [DebuggerNonUserCode]
        public void RegisterTutor(RegisterTutorRequest request) => userManager.RegisterTutor(request);

        [DebuggerNonUserCode]
        public void RegisterStudent(RegisterStudentRequest request) => userManager.RegisterStudent(request);

        public List<UserDto> GetAllUsers() => userManager.GetAllUsers();

        public AuthenticatedUserDto LoginUser(LoginRequest request) => userManager.LoginUser(request);

        public List<CareerDto> GetAllCareers() => userManager.GetAllCareers();

        public int CreateAppointment(CreateAppointmentRequest request) => appointmentManager.CreateAppointment(request.StudentId, request.TutorId, request.SessionId).TurnoId;

        public List<AppointmentDto> GetPendingAppointments() => appointmentManager.GetPendingAppointments();

        public void CancelAppointment(int appointmentId, string reason) => appointmentManager.CancelAppointment(appointmentId, reason);

        public void NoShowAppointment(int appointmentId, string reason) => appointmentManager.NoShowAppointment(appointmentId, reason);

        public void RegisterSession(int appointmentId, DateTime date, TimeSpan startTime, TimeSpan endTime) => appointmentManager.RegisterSession(appointmentId, date, startTime, endTime);

        public void EvaluateAppointment(FinalEvaluationRequest request) => appointmentManager.EvaluateAppointment(request);

        public void FinishSession(int appointmentId) => appointmentManager.FinishSession(appointmentId);

        public List<TutorDto> GetAvailableTutors() => userManager.GetAvailableTutors();

        public void SubscribeForNotifications(string studentId) => notificationManager.Subscribe(studentId, OperationContext.Current.GetCallbackChannel<INotificationCallbacks>());

        public void UnsubscribeFromNotifications(string studentId) => notificationManager.Unsubscribe(studentId);

        public List<AppointmentDto> GetAttendedAppointment(DateTime from, DateTime to) => appointmentManager.GetAttendedAppointment(from, to);

        public List<TutorDto> GetAllTutors() => userManager.GetAllTutors();

    }
}
