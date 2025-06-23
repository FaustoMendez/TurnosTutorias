using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceImplementacion.Business;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;

namespace ServiceTest
{
    [TestClass]
    public class CreateAppointmentTests
    {
        private Mock<TurnosTutoriasEntities> _ctxMock;
        private Mock<DbSet<Turnos>> _turnosMock;
        private AppointmentManager _mgr;

        [TestInitialize]
        public void Setup()
        {
            _turnosMock = new Mock<DbSet<Turnos>>();
            _ctxMock = new Mock<TurnosTutoriasEntities>();
            _ctxMock.Setup(c => c.Turnos).Returns(_turnosMock.Object);
            _mgr = new AppointmentManager(_ctxMock.Object);
        }

        [TestMethod]
        public void Should_CreateAppointment_When_NoConflict()
        {
            _ctxMock.Setup(c => c.Usuarios.Find("S1")).Returns(new Usuarios { Matricula = "S1" });
            _ctxMock.Setup(c => c.Tutores.Find("T1")).Returns(new Tutores { NumeroPersonal = "T1" });
            _ctxMock.Setup(c => c.Tutorias.Find(5)).Returns(new Tutorias { TutoriaId = 5 });

            var result = _mgr.CreateAppointment("S1", "T1", 5);

            _turnosMock.Verify(m => m.Add(It.IsAny<Turnos>()), Times.Once);
            _ctxMock.Verify(c => c.SaveChanges(), Times.Once);
            Assert.IsNotNull(result);
        }

        [TestMethod, ExpectedException(typeof(FaultException))]
        public void Should_Throw_When_ConflictExists()
        {
            var data = new[] {
                new Turnos { TurnoId = 1, Matricula = "S1", NumeroPersonal = "T2", EstadoId = 1 }
            }.AsQueryable();

            _turnosMock.As<IQueryable<Turnos>>().SetupAllProperties();
            _turnosMock.As<IQueryable<Turnos>>().Setup(m => m.Provider).Returns(data.Provider);
            _turnosMock.As<IQueryable<Turnos>>().Setup(m => m.Expression).Returns(data.Expression);
            _turnosMock.As<IQueryable<Turnos>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _turnosMock.As<IQueryable<Turnos>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            _ctxMock.Setup(c => c.Turnos).Returns(_turnosMock.Object);
            _ctxMock.Setup(c => c.Usuarios.Find("S1")).Returns(new Usuarios { Matricula = "S1" });
            _ctxMock.Setup(c => c.Tutores.Find("T1")).Returns(new Tutores { NumeroPersonal = "T1" });
            _ctxMock.Setup(c => c.Tutorias.Find(5)).Returns(new Tutorias { TutoriaId = 5 });

            _mgr.CreateAppointment("S1", "T1", 5);
        }
    }
}
