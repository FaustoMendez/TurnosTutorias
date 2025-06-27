using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Data;
using ServiceImplementacion.Business;
using ServiceContracts.DTOs;
using System.ServiceModel;
using ServiceContracts;

namespace ServiceTest
{
    internal static class TestHelperStudent
    {
        public static Mock<DbSet<T>> CreateDbSetMock<T>(IList<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mock = new Mock<DbSet<T>>();
            mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            mock.Setup(d => d.Add(It.IsAny<T>())).Callback<T>(data.Add);
            return mock;
        }
    }

    [TestClass]
    public class RegisterStudentTests
    {
        private List<Usuarios> _userData;
        private List<Carreras> _careerData;
        private Mock<DbSet<Usuarios>> _userSetMock;
        private Mock<DbSet<Carreras>> _careerSetMock;
        private Mock<TurnosTutoriasEntities> _ctxMock;
        private UserManager _mgr;

        [TestInitialize]
        public void Setup()
        {
            // Tabla Usuarios
            _userData = new List<Usuarios>();
            _userSetMock = TestHelperStudent.CreateDbSetMock(_userData);
            _userSetMock.Setup(s => s.Find(It.IsAny<object[]>()))
                        .Returns<object[]>(ids => _userData
                            .SingleOrDefault(u => u.Matricula == (string)ids[0]));

            // Tabla Carreras  (una carrera válida con ID=1)
            _careerData = new List<Carreras>
            {
                new Carreras { CarreraId = 1, Nombre = "Ing. Sistemas" }
            };
            _careerSetMock = TestHelperStudent.CreateDbSetMock(_careerData);

            // Contexto
            _ctxMock = new Mock<TurnosTutoriasEntities>();
            _ctxMock.Setup(c => c.Usuarios).Returns(_userSetMock.Object);
            _ctxMock.Setup(c => c.Carreras).Returns(_careerSetMock.Object);
            _ctxMock.Setup(c => c.SaveChanges()).Verifiable();

            _mgr = new UserManager(_ctxMock.Object);
        }

        /* ---------- Registro exitoso ------------------------------------ */
        [TestMethod]
        public void Should_RegisterStudent_When_DataValid()
        {
            var req = new RegisterStudentRequest
            {
                StudentId = "S1",
                FirstName = "Alice",
                PaternalSurname = "Smith",
                MaternalSurname = "Doe",
                Email = "alice@example.com",
                Password = "pwd123",
                CareerId = 1,
                TutorId = "T1"
            };

            _mgr.RegisterStudent(req);

            Assert.AreEqual(1, _userData.Count);
            _ctxMock.Verify(c => c.SaveChanges(), Times.Once);
        }

        /* ---------- Matrícula duplicada --------------------------------- */
        [TestMethod]
        [ExpectedException(typeof(FaultException<DuplicateStudentFault>))]
        public void Should_Throw_DuplicateStudent()
        {
            _userData.Add(new Usuarios { Matricula = "S1" });

            var req = new RegisterStudentRequest
            {
                StudentId = "S1",
                FirstName = "Bob",
                PaternalSurname = "Brown",
                MaternalSurname = "Lee",
                Email = "bob@example.com",
                Password = "pwd",
                CareerId = 1,
                TutorId = "T1"
            };

            _mgr.RegisterStudent(req);
        }

        /* ---------- Capacidad del tutor superada ------------------------ */
        [TestMethod]
        [ExpectedException(typeof(FaultException<CapacityFault>))]
        public void Should_Throw_CapacityExceeded()
        {
            for (int i = 0; i < 15; i++)
                _userData.Add(new Usuarios { Matricula = $"X{i}", TutorId = "T1" });

            var req = new RegisterStudentRequest
            {
                StudentId = "S99",
                FirstName = "Eve",
                PaternalSurname = "White",
                MaternalSurname = "Lopez",
                Email = "eve@example.com",
                Password = "pwd",
                CareerId = 1,
                TutorId = "T1"
            };

            _mgr.RegisterStudent(req);
        }
    }
}
