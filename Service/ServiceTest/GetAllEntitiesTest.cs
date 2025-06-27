using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Data;
using ServiceImplementacion.Business;
using ServiceContracts.DTOs;

namespace ServiceTest
{
    internal static class EntityTestHelper
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
    }

    [TestClass]
    public class GetAllEntitiesTests
    {
        private List<Usuarios> _userData;
        private List<Tutores> _tutorData;
        private List<Carreras> _careerData;

        private Mock<TurnosTutoriasEntities> _ctxMock;
        private Mock<DbSet<Usuarios>> _userSetMock;
        private Mock<DbSet<Tutores>> _tutorSetMock;
        private Mock<DbSet<Carreras>> _careerSetMock;

        private UserManager _userMgr;

        [TestInitialize]
        public void Setup()
        {
            _userData = new List<Usuarios>
            {
                new Usuarios { Matricula = "S1", Nombre = "Ana",  ApellidoPaterno = "López", ApellidoMaterno = "Ramos", Email = "ana@example.com", CarreraId = 1, TutorId = "T1" },
                new Usuarios { Matricula = "S2", Nombre = "Juan", ApellidoPaterno = "Pérez", ApellidoMaterno = "García", Email = "juan@example.com", CarreraId = 2, TutorId = "T2" }
            };
            _tutorData = new List<Tutores>
            {
                new Tutores { NumeroPersonal = "T1", Nombre = "Laura", ApellidoPaterno = "Martínez", ApellidoMaterno = "Suárez", Telefono = "555-1111", Email = "laura@example.com" },
                new Tutores { NumeroPersonal = "T2", Nombre = "Carlos", ApellidoPaterno = "Hernández", ApellidoMaterno = "Núñez", Telefono = "555-2222", Email = "carlos@example.com" }
            };
            _careerData = new List<Carreras>
            {
                new Carreras { CarreraId = 1, Nombre = "Ingeniería" },
                new Carreras { CarreraId = 2, Nombre = "Derecho" }
            };

            _userSetMock = EntityTestHelper.MockDbSet(_userData);
            _tutorSetMock = EntityTestHelper.MockDbSet(_tutorData);
            _careerSetMock = EntityTestHelper.MockDbSet(_careerData);

            _ctxMock = new Mock<TurnosTutoriasEntities>();
            _ctxMock.Setup(c => c.Usuarios).Returns(_userSetMock.Object);
            _ctxMock.Setup(c => c.Tutores).Returns(_tutorSetMock.Object);
            _ctxMock.Setup(c => c.Carreras).Returns(_careerSetMock.Object);

            _userMgr = new UserManager(_ctxMock.Object);
        }

        [TestMethod]
        public void GetAllUsers_Should_ReturnMappedDtos()
        {
            var list = _userMgr.GetAllUsers();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("S1", list[0].UserId);
            Assert.AreEqual("Ana", list[0].FirstName);
            Assert.AreEqual("López", list[0].PaternalSurname);
        }

        [TestMethod]
        public void GetAllTutors_Should_ReturnMappedDtos()
        {
            var list = _userMgr.GetAllTutors();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("T1", list[0].TutorId);
            Assert.AreEqual("Laura", list[0].FirstName);
        }

        [TestMethod]
        public void GetAllCareers_Should_ReturnMappedDtos()
        {
            var list = _userMgr.GetAllCareers();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(1, list[0].CareerId);
            Assert.AreEqual("Ingeniería", list[0].CareerName);
        }
    }
}
