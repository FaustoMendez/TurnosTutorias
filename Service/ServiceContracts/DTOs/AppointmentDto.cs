using System;
using System.Runtime.Serialization;

namespace ServiceContracts.DTOs
{
    using System;
    using System.Runtime.Serialization;

    namespace ServiceContracts.DTOs
    {
        [DataContract]
        public class AppointmentDto
        {
            [DataMember] public int AppointmentId { get; set; }
            [DataMember] public string StudentId { get; set; }
            [DataMember] public string StudentName { get; set; }
            [DataMember] public string TutorId { get; set; }
            [DataMember] public string SessionName { get; set; }
            [DataMember] public DateTime SessionDate { get; set; }
            [DataMember] public TimeSpan SessionStart { get; set; }
            [DataMember] public TimeSpan SessionEnd { get; set; }
            [DataMember] public string Status { get; set; }
        }
    }
}
