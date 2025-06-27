using System;
using System.Linq;
using System.ServiceModel;
using System.Security.Cryptography;
using System.Text;
using ServiceContracts.DTOs;
using Data;
using System.Collections.Generic;
using ServiceContracts;
using System.Diagnostics;

namespace ServiceImplementacion.Business
{
    public class UserManager
    {
        private readonly TurnosTutoriasEntities _context;
        public UserManager(TurnosTutoriasEntities context) => _context = context;

        [DebuggerNonUserCode]
        public void RegisterStudent(RegisterStudentRequest request)
        {
            if (_context.Usuarios.Any(u => u.Matricula == request.StudentId))
                throw new FaultException<DuplicateStudentFault>(
                    new DuplicateStudentFault { Message = ServiceResources.DuplicateMatricula_Message },
                    new FaultReason(ServiceResources.DuplicateMatricula_Reason));

            var assignedCount = _context.Usuarios.Count(u => u.TutorId == request.TutorId);
            if (assignedCount >= 15)
                throw new FaultException<CapacityFault>(
                    new CapacityFault { Message = ServiceResources.TutorCapacityFault_Message },
                    new FaultReason(ServiceResources.TutorCapacityFault_Reason));

            byte[] hash;
            using (var sha = SHA256.Create())
                hash = sha.ComputeHash(Encoding.UTF8.GetBytes(request.Password));

            _context.Usuarios.Add(new Usuarios
            {
                Matricula = request.StudentId,
                Nombre = request.FirstName,
                ApellidoPaterno = request.PaternalSurname,
                ApellidoMaterno = request.MaternalSurname,
                Email = request.Email,
                PasswordHash = hash,
                CarreraId = request.CareerId,
                TutorId = request.TutorId,
                FechaRegistro = DateTime.Now
            });
            _context.SaveChanges();
        }

        [DebuggerNonUserCode]
        public void RegisterTutor(RegisterTutorRequest request)
        {
            if (_context.Tutores.Any(t => t.NumeroPersonal == request.TutorId))
                throw new FaultException<DuplicateTutorFault>(
                    new DuplicateTutorFault { Message = ServiceResources.DuplicateTutorFault_Message },
                    new FaultReason(ServiceResources.DuplicateTutorFault_Reason));

            byte[] hash;
            using (var sha = SHA256.Create())
                hash = sha.ComputeHash(Encoding.UTF8.GetBytes(request.Password));

            _context.Tutores.Add(new Tutores
            {
                NumeroPersonal = request.TutorId,
                Nombre = request.FirstName,
                ApellidoPaterno = request.PaternalSurname,
                ApellidoMaterno = request.MaternalSurname,
                Telefono = request.Phone,
                Email = request.Email,
                PasswordHash = hash
            });
            _context.SaveChanges();
        }

        public List<TutorDto> GetAvailableTutors()
        {
            return _context.Tutores
                .Select(t => new
                {
                    Tutor = t,
                    AssignedCount = _context.Usuarios.Count(u => u.TutorId == t.NumeroPersonal)
                })
                .Where(x => x.AssignedCount < 15)
                .Select(x => new TutorDto
                {
                    TutorId = x.Tutor.NumeroPersonal,
                    FirstName = x.Tutor.Nombre,
                    PaternalSurname = x.Tutor.ApellidoPaterno,
                    MaternalSurname = x.Tutor.ApellidoMaterno,
                    Phone = x.Tutor.Telefono,
                    Email = x.Tutor.Email
                })
                .ToList();
        }

        public List<UserDto> GetAllUsers() 
            => _context.Usuarios
        .Select(u => new UserDto
        {
            UserId = u.Matricula,
            FirstName = u.Nombre,
            PaternalSurname = u.ApellidoPaterno,
            MaternalSurname = u.ApellidoMaterno,
            Email = u.Email,
            CareerId = (int)u.CarreraId,
            TutorId = u.TutorId
        })
        .ToList();


        public AuthenticatedUserDto LoginUser(LoginRequest request)
        {
            var estudiante = _context.Usuarios.Find(request.StudentId);
            if (estudiante != null)
                return AuthenticateStudent(estudiante, request.Password);

            var tutor = _context.Tutores.Find(request.StudentId);
            if (tutor != null)
                return AuthenticateTutor(tutor, request.Password);

            throw new FaultException("Usuario no encontrado.");
        }

        private AuthenticatedUserDto AuthenticateStudent(Usuarios u, string plainPwd)
        {
            var hash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(plainPwd));
            if (!u.PasswordHash.SequenceEqual(hash))
                throw new FaultException("Contraseña incorrecta.");

            return new AuthenticatedUserDto
            {
                Id = u.Matricula,
                FirstName = u.Nombre,
                PaternalSurname = u.ApellidoPaterno,
                MaternalSurname = u.ApellidoMaterno,
                Email = u.Email,
                CareerId = u.CarreraId ?? 0,
                Role = "Student"
            };
        }

        private AuthenticatedUserDto AuthenticateTutor(Tutores t, string plainPwd)
        {
            var hash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(plainPwd));
            if (!t.PasswordHash.SequenceEqual(hash))
                throw new FaultException("Contraseña incorrecta.");

            return new AuthenticatedUserDto
            {
                Id = t.NumeroPersonal,
                FirstName = t.Nombre,
                PaternalSurname = t.ApellidoPaterno,
                MaternalSurname = t.ApellidoMaterno,
                Phone = t.Telefono,
                Email = t.Email,
                CareerId = 0,              
                Role = "Tutor"
            };
        }

        public List<TutorDto> GetAllTutors()
        {
            return _context.Tutores
                .Select(t => new TutorDto
                {
                    TutorId = t.NumeroPersonal,
                    FirstName = t.Nombre,
                    PaternalSurname = t.ApellidoPaterno,
                    MaternalSurname = t.ApellidoMaterno,
                    Phone = t.Telefono,
                    Email = t.Email
                })
                .ToList();
        }

        public List<CareerDto> GetAllCareers()
        {
            return _context.Carreras
            .Select(c => new CareerDto
            {
               CareerId = c.CarreraId,    
               CareerName = c.Nombre          
            })
            .ToList();
        }
    }
}



