using System;
using System.Collections.Concurrent;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using ServiceContracts;
using ServiceContracts.DTOs;

namespace ServiceImplementacion.Business
{
    public class NotificationManager : IDisposable
    {
        private readonly AppointmentManager _appointmentMgr;
        private readonly ConcurrentDictionary<string, INotificationCallback> _subs
            = new ConcurrentDictionary<string, INotificationCallback>();
        private readonly Timer _timer;

        public NotificationManager(AppointmentManager appointmentMgr)
        {
            _appointmentMgr = appointmentMgr;
            _timer = new Timer(_ => CheckAndNotify(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        public void Subscribe(string studentId, INotificationCallback callback)
        {
            _subs[studentId] = callback;
        }

        public void Unsubscribe(string studentId)
        {
            _subs.TryRemove(studentId, out _);
        }

        private void CheckAndNotify()
        {
            var now = DateTime.Now;
            var upcoming = _appointmentMgr
                .GetPendingAppointments()  
                .Where(a =>
                    a.SessionDate >= now &&
                    a.SessionDate <= now.AddMinutes(5));

            foreach (var appt in upcoming)
            {
                if (_subs.TryGetValue(appt.StudentId, out var cb))
                {
                    try
                    {
                        cb.NotifyUpcomingAppointment(appt);
                    }
                    catch
                    {
                        _subs.TryRemove(appt.StudentId, out _);
                    }
                }
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
            _subs.Clear();
        }
    }
}
