using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using ServiceContracts.DTOs;
using Data;
using ServiceContracts.DTOs.ServiceContracts.DTOs;

namespace ServiceImplementacion.Business
{
    public class AppointmentManager
    {
        private readonly TurnosTutoriasEntities _ctx;
        public AppointmentManager(TurnosTutoriasEntities ctx) => _ctx = ctx;

        public Turnos CreateAppointment(string studentId, string tutorId, int sessionId)
        {
            var student = _ctx.Usuarios.Find(studentId)
                          ?? throw new FaultException("El estudiante no existe.");
            var tutor = _ctx.Tutores.Find(tutorId)
                       ?? throw new FaultException("El tutor no existe.");
            var session = _ctx.Tutorias.Find(sessionId)
                        ?? throw new FaultException("La tutoría no existe.");

            bool conflict = _ctx.Turnos.Any(t =>
                t.EstadoId == 1 &&
                (t.Matricula == studentId || t.NumeroPersonal == tutorId));
            if (conflict)
                throw new FaultException("Ya existe una cita pendiente en conflicto.");

            var appointment = new Turnos
            {
                Matricula = studentId,
                NumeroPersonal = tutorId,
                TutoriaId = sessionId,
                EstadoId = 1,              // Pendiente
                FechaCreacion = DateTime.Now
            };
            _ctx.Turnos.Add(appointment);
            _ctx.SaveChanges();
            return appointment;
        }

        public List<AppointmentDto> GetPendingAppointments()
        {
            return _ctx.Turnos
                .Where(t => t.EstadoId == 1)
                .Select(t => new AppointmentDto
                {
                    AppointmentId = t.TurnoId,
                    StudentId = t.Matricula,
                    StudentName = t.Usuarios.Nombre + " " + t.Usuarios.ApellidoPaterno,
                    TutorId = t.NumeroPersonal,
                    SessionName = t.Tutorias.Nombre,
                    SessionDate = t.Tutorias.Fecha,
                    SessionStart = t.Tutorias.HoraInicio,
                    SessionEnd = t.Tutorias.HoraFin,
                    Status = t.TurnoEstados.EstadoName
                })
                .ToList();
        }

        public void CancelAppointment(int appointmentId, string reason)
        {
            var appt = _ctx.Turnos.Find(appointmentId)
                      ?? throw new FaultException("La cita no existe.");
            appt.EstadoId = 4; // Cancelado
            _ctx.TurnoCancelaciones.Add(new TurnoCancelaciones
            {
                TurnoId = appointmentId,
                FechaCancelacion = DateTime.Now.Date,
                HoraCancelacion = DateTime.Now.TimeOfDay,
                Razon = reason
            });
            _ctx.SaveChanges();
        }

        public void NoShowAppointment(int appointmentId, string reason)
        {
            var appt = _ctx.Turnos.Find(appointmentId)
                      ?? throw new FaultException("La cita no existe.");
            appt.EstadoId = 3; // NoAtendido
            _ctx.TurnoAbandonos.Add(new TurnoAbandonos
            {
                TurnoId = appointmentId,
                FechaAbandono = DateTime.Now.Date,
                HoraAbandono = DateTime.Now.TimeOfDay,
                Razon = reason
            });
            _ctx.SaveChanges();
        }

        public void EvaluateAppointment(FinalEvaluationRequest dto)
        {
            _ctx.EvaluacionFinal.Add(new EvaluacionFinal
            {
                TutoriaId = dto.SessionId,
                Matricula = dto.StudentId,
                NumeroPersonal = dto.TutorId,
                Comentario = dto.Comment,
                Calificacion = dto.Rating,            
                FechaEval = DateTime.Now
            });
            _ctx.SaveChanges();
        }

        public void RegisterSession(int appointmentId, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            var appt = _ctx.Turnos.Find(appointmentId)
                       ?? throw new FaultException("La cita no existe.");

            var session = new Tutorias
            {
                Fecha = date,
                HoraInicio = startTime,
                HoraFin = endTime,
                DuracionMinutos = (int)(endTime - startTime).TotalMinutes
            };
            _ctx.Tutorias.Add(session);
            _ctx.SaveChanges();  

            appt.TutoriaId = session.TutoriaId;
            appt.EstadoId = 2; 
            _ctx.SaveChanges();
        }

        public void FinishSession(int appointmentId)
        {
            var appt = _ctx.Turnos.Find(appointmentId)
                       ?? throw new FaultException("Turno no existe.");

            appt.EstadoId = 2;
            _ctx.SaveChanges();
        }

        public List<AppointmentDto> GetAttendedAppointment(DateTime from, DateTime to)
        {
            return _ctx.Turnos
                       .Where(t => t.EstadoId == 2                      // 2 = “Atendido
                                && t.FechaCreacion >= from
                                && t.FechaCreacion <= to)
                       .Select(t => new AppointmentDto
                       {
                           AppointmentId = t.TurnoId,
                           StudentId = t.Matricula,
                           TutorId = t.NumeroPersonal,
                           SessionDate = t.FechaCreacion,
                           Status = t.TurnoEstados.EstadoName
                       })
                       .ToList();
        }
    }
}
