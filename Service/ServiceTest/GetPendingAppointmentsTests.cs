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
    public static class TestHelpers
    {
        public static Mock<DbSet<T>> CreateDbSetMock<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();
            var dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            dbSetMock.Setup(d => d.Add(It.IsAny<T>()))
                     .Callback<T>(sourceList.Add);

            return dbSetMock;
        }
    }

    [TestClass]
    public class GetPendingAppointmentsTests
    {
        private List<Turnos> _data;
        private Mock<DbSet<Turnos>> _turnosMock;
        private Mock<TurnosTutoriasEntities> _ctxMock;
        private AppointmentManager _mgr;

        [TestInitialize]
        public void Setup()
        {
            _data = new List<Turnos>();
            _turnosMock = TestHelperStudent.CreateDbSetMock(_data);
            _ctxMock = new Mock<TurnosTutoriasEntities>();
            _ctxMock.Setup(c => c.Turnos).Returns(_turnosMock.Object);
            _mgr = new AppointmentManager(_ctxMock.Object);
        }

        [TestMethod]
        public void Should_ReturnEmpty_When_NoPending()
        {
            var list = _mgr.GetPendingAppointments();
            Assert.AreEqual(0, list.Count);
        }
    }
}
