using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Data;
using ServiceImplementacion.Business;
using ServiceContracts.DTOs;
using System.ServiceModel;

namespace ServiceTest
{
    internal static class LoginTestHelper
    {
        public static Mock<DbSet<T>> MockDbSet<T>(IList<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mock = new Mock<DbSet<T>>();
            mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            return mock;
        }

        public static byte[] Sha256(string plain)
            => SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(plain));
    }

    [TestClass]
    public class LoginUserTests
    {
        private List<Usuarios> _studentData;
        private List<Tutores> _tutorData;
        private Mock<DbSet<Usuarios>> _studentSetMock;
        private Mock<DbSet<Tutores>> _tutorSetMock;
        private Mock<TurnosTutoriasEntities> _ctxMock;
        private UserManager _mgr;

        [TestInitialize]
        public void Setup()
        {
            _studentData = new List<Usuarios>();
            _tutorData = new List<Tutores>();

            _studentSetMock = LoginTestHelper.MockDbSet(_studentData);
            _tutorSetMock = LoginTestHelper.MockDbSet(_tutorData);

            _studentSetMock.Setup(s => s.Find(It.IsAny<object[]>()))
                           .Returns<object[]>(ids => _studentData.SingleOrDefault(u => u.Matricula == (string)ids[0]));
            _tutorSetMock.Setup(s => s.Find(It.IsAny<object[]>()))
                         .Returns<object[]>(ids => _tutorData.SingleOrDefault(t => t.NumeroPersonal == (string)ids[0]));

            _ctxMock = new Mock<TurnosTutoriasEntities>();
            _ctxMock.Setup(c => c.Usuarios).Returns(_studentSetMock.Object);
            _ctxMock.Setup(c => c.Tutores).Returns(_tutorSetMock.Object);

            _mgr = new UserManager(_ctxMock.Object);

            _studentData.Add(new Usuarios
            {
                Matricula = "S1",
                Nombre = "Ana",
                ApellidoPaterno = "Lopez",
                ApellidoMaterno = "Ramos",
                Email = "ana@example.com",
                CarreraId = 1,
                PasswordHash = LoginTestHelper.Sha256("pwd123")
            });

            _tutorData.Add(new Tutores
            {
                NumeroPersonal = "T1",
                Nombre = "Laura",
                ApellidoPaterno = "Martinez",
                ApellidoMaterno = "Suarez",
                Telefono = "555-1111",
                Email = "laura@example.com",
                PasswordHash = LoginTestHelper.Sha256("pwd456")
            });
        }

        [TestMethod]
        public void Login_Should_ReturnStudentDto_When_CredentialsValid()
        {
            var req = new LoginRequest { StudentId = "S1", Password = "pwd123" };
            var dto = _mgr.LoginUser(req);

            Assert.IsNotNull(dto);
            Assert.AreEqual("S1", dto.Id);
            Assert.AreEqual("Student", dto.Role);
        }

        [TestMethod]
        public void Login_Should_ReturnTutorDto_When_CredentialsValid()
        {
            var req = new LoginRequest { StudentId = "T1", Password = "pwd456" };
            var dto = _mgr.LoginUser(req);

            Assert.IsNotNull(dto);
            Assert.AreEqual("T1", dto.Id);
            Assert.AreEqual("Tutor", dto.Role);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void Login_Should_Throw_When_WrongPassword()
        {
            var req = new LoginRequest { StudentId = "S1", Password = "wrong" };
            _mgr.LoginUser(req);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void Login_Should_Throw_When_UserNotFound()
        {
            var req = new LoginRequest { StudentId = "Z9", Password = "pwd" };
            _mgr.LoginUser(req);
        }
    }
}
