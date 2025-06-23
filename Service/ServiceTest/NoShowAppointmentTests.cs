using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceImplementacion.Business;
using Data;
using System.Data.Entity;

namespace ServiceImplementacion.Tests
{
    [TestClass]
    public class NoShowAppointmentTests
    {
        private Mock<TurnosTutoriasEntities> _ctxMock;
        private Mock<DbSet<Turnos>> _turnosMock;
        private Mock<DbSet<TurnoAbandonos>> _noShowMock;
        private AppointmentManager _mgr;

        [TestInitialize]
        public void Setup()
        {
            _turnosMock = new Mock<DbSet<Turnos>>();
            _noShowMock = new Mock<DbSet<TurnoAbandonos>>();
            _ctxMock = new Mock<TurnosTutoriasEntities>();
            _ctxMock.Setup(c => c.Turnos).Returns(_turnosMock.Object);
            _ctxMock.Setup(c => c.TurnoAbandonos).Returns(_noShowMock.Object);
            _mgr = new AppointmentManager(_ctxMock.Object);
        }

        [TestMethod]
        public void Should_NoShowAndRecord_When_ValidId()
        {
            var turno = new Turnos { TurnoId = 4, EstadoId = 1 };
            _turnosMock.Setup(m => m.Find(4)).Returns(turno);

            _mgr.NoShowAppointment(4, "No show");

            Assert.AreEqual(3, turno.EstadoId);
            _noShowMock.Verify(m => m.Add(It.IsAny<TurnoAbandonos>()), Times.Once);
            _ctxMock.Verify(c => c.SaveChanges(), Times.Once);
        }

        [TestMethod, ExpectedException(typeof(FaultException))]
        public void Should_Throw_When_InvalidId()
        {
            _mgr.NoShowAppointment(100, "X");
        }
    }
}
