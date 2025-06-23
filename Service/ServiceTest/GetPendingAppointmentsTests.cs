using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceImplementacion.Business;
using ServiceContracts.DTOs;
using Data;

namespace ServiceTest
{
    [TestClass]
    public class GetPendingAppointmentsTests
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
        public void Should_ReturnDtos_When_PendingExists()
        {
            var now = DateTime.UtcNow;
            var data = new List<Turnos> {
                new Turnos {
                    TurnoId      = 2,
                    Matricula    = "S2",
                    NumeroPersonal = "T2",
                    FechaCreacion  = now,
                    EstadoId       = 1,
                    TurnoEstados   = new TurnoEstados { EstadoName = "Pendiente" }
                }
            }.AsQueryable();

            _turnosMock.As<IQueryable<Turnos>>().SetupAllProperties();
            _turnosMock.As<IQueryable<Turnos>>().Setup(m => m.Provider).Returns(data.Provider);
            _turnosMock.As<IQueryable<Turnos>>().Setup(m => m.Expression).Returns(data.Expression);
            _turnosMock.As<IQueryable<Turnos>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _turnosMock.As<IQueryable<Turnos>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var list = _mgr.GetPendingAppointments();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(2, list[0].AppointmentId);
            Assert.AreEqual("Pendiente", list[0].Status);
        }

        [TestMethod]
        public void Should_ReturnEmpty_When_NoPending()
        {
            // default empty setup
            var list = _mgr.GetPendingAppointments();
            Assert.AreEqual(0, list.Count);
        }
    }
}
