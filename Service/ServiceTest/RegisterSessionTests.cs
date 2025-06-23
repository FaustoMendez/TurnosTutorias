using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceImplementacion.Business;
using Data;
using System.Data.Entity;
using System.ServiceModel;

namespace ServiceTests
{
    [TestClass]
    public class RegisterSessionTests
    {
        private Mock<TurnosTutoriasEntities> _ctxMock;
        private Mock<DbSet<Turnos>> _turnosMock;
        private Mock<DbSet<Tutorias>> _sessionsMock;
        private AppointmentManager _mgr;

        [TestInitialize]
        public void Setup()
        {
            _turnosMock = new Mock<DbSet<Turnos>>();
            _sessionsMock = new Mock<DbSet<Tutorias>>();
            _ctxMock = new Mock<TurnosTutoriasEntities>();
            _ctxMock.Setup(c => c.Turnos).Returns(_turnosMock.Object);
            _ctxMock.Setup(c => c.Tutorias).Returns(_sessionsMock.Object);
            _mgr = new AppointmentManager(_ctxMock.Object);
        }

        [TestMethod]
        public void Should_CreateSessionAndAttach_When_ValidId()
        {
            var turno = new Turnos { TurnoId = 5, EstadoId = 1 };
            _turnosMock.Setup(m => m.Find(5)).Returns(turno);

            _mgr.RegisterSession(5, DateTime.Today, TimeSpan.FromHours(8), TimeSpan.FromHours(9));

            _sessionsMock.Verify(m => m.Add(It.IsAny<Tutorias>()), Times.Once);
            Assert.AreEqual(2, turno.EstadoId);
            _ctxMock.Verify(c => c.SaveChanges(), Times.AtLeastOnce);
        }

        [TestMethod, ExpectedException(typeof(FaultException))]
        public void Should_Throw_When_InvalidId()
        {
            _mgr.RegisterSession(999, DateTime.Today, TimeSpan.Zero, TimeSpan.Zero);
        }
    }
}
