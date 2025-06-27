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
    internal static class TestHelperTutors
    {
        public static Mock<DbSet<Tutores>> CreateDbSetMock(IList<Tutores> data)
        {
            var queryable = data.AsQueryable();
            var mock = new Mock<DbSet<Tutores>>();

            mock.As<IQueryable<Tutores>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mock.As<IQueryable<Tutores>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mock.As<IQueryable<Tutores>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mock.As<IQueryable<Tutores>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            mock.Setup(d => d.Add(It.IsAny<Tutores>())).Callback<Tutores>(data.Add);

            return mock;
        }
    }

    [TestClass]
    public class RegisterTutorTests
    {
        private List<Tutores> _tutorData;
        private Mock<DbSet<Tutores>> _tutorSetMock;
        private Mock<TurnosTutoriasEntities> _ctxMock;
        private UserManager _mgr;   

        [TestInitialize]
        public void Setup()
        {
            _tutorData = new List<Tutores>();
            _tutorSetMock = TestHelperTutors.CreateDbSetMock(_tutorData);

            _ctxMock = new Mock<TurnosTutoriasEntities>();
            _ctxMock.Setup(c => c.Tutores).Returns(_tutorSetMock.Object);
            _ctxMock.Setup(c => c.SaveChanges()).Verifiable();

            _mgr = new UserManager(_ctxMock.Object);
        }

        [TestMethod]
        public void Should_RegisterTutor_When_DataValid()
        {
            var req = new RegisterTutorRequest
            {
                TutorId = "T1",
                FirstName = "John",
                PaternalSurname = "Doe",
                MaternalSurname = "Smith",
                Email = "john.doe@example.com",
                Phone = "555-0000",
                Password = "pwd123"
            };

            _mgr.RegisterTutor(req);

            Assert.AreEqual(1, _tutorData.Count);
            _ctxMock.Verify(c => c.SaveChanges(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException<DuplicateTutorFault>))]
        public void Should_Throw_DuplicateTutor()
        {
            _tutorData.Add(new Tutores { NumeroPersonal = "T1" });

            var req = new RegisterTutorRequest
            {
                TutorId = "T1",
                FirstName = "Jane",
                PaternalSurname = "Roe",
                MaternalSurname = "Lee",
                Email = "jane@example.com",
                Phone = "555-1111",
                Password = "pwd"
            };

            _mgr.RegisterTutor(req);
        }
    }
}
