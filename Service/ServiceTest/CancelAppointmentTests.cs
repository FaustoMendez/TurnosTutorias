using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceImplementacion.Business;
using Data;
using System.Data.Entity;

namespace ServiceTests
{
    [TestClass]
    public class CancelAppointmentTests
    {
        private Mock<TurnosTutoriasEntities> _ctxMock;
        private Mock<DbSet<Turnos>> _turnosMock;
        private Mock<DbSet<TurnoCancelaciones>> _cancelMock;
        private AppointmentManager _mgr;

        [TestInitialize]
        public void Setup()
        {
            _turnosMock = new Mock<DbSet<Turnos>>();
            _cancelMock = new Mock<DbSet<TurnoCancelaciones>>();
            _ctxMock = new Mock<TurnosTutoriasEntities>();
            _ctxMock.Setup(c => c.Turnos).Returns(_turnosMock.Object);
            _ctxMock.Setup(c => c.TurnoCancelaciones).Returns(_cancelMock.Object);
            _mgr = new AppointmentManager(_ctxMock.Object);
        }

        [TestMethod]
        public void Should_CancelAndRecord_When_ValidId()
        {
            var turno = new Turnos { TurnoId = 3, EstadoId = 1 };
            _turnosMock.Setup(m => m.Find(3)).Returns(turno);

            _mgr.CancelAppointment(3, "Reason");

            Assert.AreEqual(4, turno.EstadoId);
            _cancelMock.Verify(m => m.Add(It.IsAny<TurnoCancelaciones>()), Times.Once);
            _ctxMock.Verify(c => c.SaveChanges(), Times.Once);
        }

        [TestMethod, ExpectedException(typeof(FaultException))]
        public void Should_Throw_When_InvalidId()
        {
            _mgr.CancelAppointment(99, "X");
        }
    }
}
