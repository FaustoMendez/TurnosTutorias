using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceImplementacion.Business;
using ServiceContracts.DTOs;
using Data;
using System.Data.Entity;

namespace ServiceTests
{
    [TestClass]
    public class EvaluateAppointmentTests
    {
        private Mock<TurnosTutoriasEntities> _ctxMock;
        private Mock<DbSet<EvaluacionFinal>> _evalMock;
        private AppointmentManager _mgr;

        [TestInitialize]
        public void Setup()
        {
            _evalMock = new Mock<DbSet<EvaluacionFinal>>();
            _ctxMock = new Mock<TurnosTutoriasEntities>();
            _ctxMock.Setup(c => c.EvaluacionFinal).Returns(_evalMock.Object);
            _mgr = new AppointmentManager(_ctxMock.Object);
        }

        [TestMethod]
        public void Should_EvaluateAndRecord()
        {
            _mgr.EvaluateAppointment(new FinalEvaluationRequest
            {
                SessionId = 7,
                StudentId = "S7",
                TutorId = "T7",
                Comment = "Good",
                AttendanceStatus = 'A',
                Reason = null
            });

            _evalMock.Verify(m => m.Add(It.IsAny<EvaluacionFinal>()), Times.Once);
            _ctxMock.Verify(c => c.SaveChanges(), Times.Once);
        }
    }
}
