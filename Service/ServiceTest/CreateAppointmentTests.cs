using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Data;
using ServiceImplementacion.Business;
using System.ServiceModel;

namespace ServiceTest
{
    internal static class TestHelper
    {
        public static Mock<DbSet<T>> CreateDbSetMock<T>(IList<T> source)
            where T : class
        {
            var queryable = source.AsQueryable();

            var dbSetMock = new Mock<DbSet<T>>();
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            dbSetMock.Setup(m => m.Add(It.IsAny<T>()))
                     .Callback<T>(source.Add);

            return dbSetMock;
        }
    }

    [TestClass]
    public class CreateAppointmentTests
    {
        private List<Usuarios> _usuarioData;
        private List<Tutores> _tutorData;
        private List<Tutorias> _sesionData;
        private List<Turnos> _turnoData;
        private Mock<DbSet<Usuarios>> _usuarioSetMock;
        private Mock<DbSet<Tutores>> _tutorSetMock;
        private Mock<DbSet<Tutorias>> _sesionSetMock;
        private Mock<DbSet<Turnos>> _turnoSetMock;
        private Mock<TurnosTutoriasEntities> _ctxMock;
        private AppointmentManager _mgr;

        [TestInitialize]
        public void Setup()
        {
            _usuarioData = new List<Usuarios>
            {
                new Usuarios { Matricula = "S1", Nombre = "Alice"  },
                new Usuarios { Matricula = "S2", Nombre = "Bob"    }
            };

            _tutorData = new List<Tutores>
            {
                new Tutores { NumeroPersonal = "T1", Nombre = "Prof X" },
                new Tutores { NumeroPersonal = "T2", Nombre = "Prof Y" }
            };

            _sesionData = new List<Tutorias>
            {
                new Tutorias { TutoriaId = 5, Nombre = "Álgebra" }
            };

            _turnoData = new List<Turnos>();      

            _usuarioSetMock = TestHelper.CreateDbSetMock(_usuarioData);
            _tutorSetMock = TestHelper.CreateDbSetMock(_tutorData);
            _sesionSetMock = TestHelper.CreateDbSetMock(_sesionData);
            _turnoSetMock = TestHelper.CreateDbSetMock(_turnoData);
            _usuarioSetMock.Setup(s => s.Find(It.IsAny<object[]>()))
                           .Returns<object[]>(ids => _usuarioData
                               .SingleOrDefault(u => u.Matricula == (string)ids[0]));

            _tutorSetMock.Setup(s => s.Find(It.IsAny<object[]>()))
                           .Returns<object[]>(ids => _tutorData
                               .SingleOrDefault(t => t.NumeroPersonal == (string)ids[0]));

            _sesionSetMock.Setup(s => s.Find(It.IsAny<object[]>()))
                           .Returns<object[]>(ids => _sesionData
                               .SingleOrDefault(ses => ses.TutoriaId == (int)ids[0]));

            _ctxMock = new Mock<TurnosTutoriasEntities>();
            _ctxMock.Setup(c => c.Usuarios).Returns(_usuarioSetMock.Object);
            _ctxMock.Setup(c => c.Tutores).Returns(_tutorSetMock.Object);
            _ctxMock.Setup(c => c.Tutorias).Returns(_sesionSetMock.Object);
            _ctxMock.Setup(c => c.Turnos).Returns(_turnoSetMock.Object);
            _ctxMock.Setup(c => c.SaveChanges()).Verifiable();

            _mgr = new AppointmentManager(_ctxMock.Object);
        }

        [TestMethod]
        public void Should_CreateAppointment_When_NoConflict()
        {
            var created = _mgr.CreateAppointment("S1", "T1", 5);

            Assert.IsNotNull(created);
            Assert.AreEqual("S1", created.Matricula);
            Assert.AreEqual("T1", created.NumeroPersonal);
            Assert.AreEqual(1, created.EstadoId);      

            _turnoSetMock.Verify(d => d.Add(It.IsAny<Turnos>()), Times.Once);
            _ctxMock.Verify(c => c.SaveChanges(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void Should_Throw_When_ConflictExists()
        {
            _turnoData.Add(new Turnos
            {
                TurnoId = 99,
                Matricula = "S1",       
                NumeroPersonal = "T2",
                EstadoId = 1           
            });

            _mgr.CreateAppointment("S1", "T1", 5);
        }
    }
}
