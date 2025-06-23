using System;
using System.Linq;
using System.ServiceModel;
using System.Security.Cryptography;
using System.Text;
using ServiceContracts.DTOs;
using Data;
using System.Collections.Generic;

namespace ServiceImplementacion.Business
{
    public class UserManager
    {
        private readonly TurnosTutoriasEntities _context;
        public UserManager(TurnosTutoriasEntities context) => _context = context;

        public void RegisterStudent(RegisterStudentRequest request)
        {
            if (_context.Usuarios.Any(u => u.Matricula == request.StudentId))
                throw new FaultException("Ya existe un estudiante con esa matrícula.");
            if (_context.Usuarios.Any(u => u.Email == request.Email))
                throw new FaultException("El correo ya está registrado.");
            if (!_context.Carreras.Any(c => c.CarreraId == request.MajorId))
                throw new FaultException("La carrera indicada no existe.");
            if (!_context.Tutores.Any(t => t.NumeroPersonal == request.TutorId))
                throw new FaultException("El tutor indicado no existe.");

            var assignedCount = _context.Usuarios.Count(u => u.TutorId == request.TutorId);
            if (assignedCount >= 15)
                throw new FaultException("El tutor ya tiene 15 estudiantes asignados.");

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
                CarreraId = request.MajorId,
                TutorId = request.TutorId,
                FechaRegistro = DateTime.Now
            });
            _context.SaveChanges();
        }

        public void RegisterTutor(RegisterTutorRequest request)
        {
            if (_context.Tutores.Any(t => t.NumeroPersonal == request.TutorId))
                throw new FaultException("Ya existe un tutor con ese número de personal.");
            if (_context.Tutores.Any(t => t.Email == request.Email))
                throw new FaultException("El correo ya está registrado.");

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
            var tutors = _context.Tutores
                .Select(t => new {
                    t.NumeroPersonal,
                    t.Nombre,
                    t.ApellidoPaterno,
                    t.ApellidoMaterno,
                    Load = _context.Usuarios.Count(u => u.TutorId == t.NumeroPersonal)
                })
                .Where(x => x.Load < 15)
                .ToList();

            return tutors
                .Select(x => new TutorDto
                {
                    TutorId = x.NumeroPersonal,
                    FullName = $"{x.Nombre} {x.ApellidoPaterno} {x.ApellidoMaterno}",
                    CurrentLoad = x.Load
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
            MajorId = u.TutorId     
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
                MajorId = u.CarreraId ?? 0,
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
                Email = t.Email,
                MajorId = 0,              
                Role = "Tutor"
            };
        }
    }
}



