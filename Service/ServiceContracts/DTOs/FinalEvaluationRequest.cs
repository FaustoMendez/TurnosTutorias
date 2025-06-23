using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTOs
{
    [DataContract]
    public class FinalEvaluationRequest
    {
        [DataMember] public int SessionId { get; set; }
        [DataMember] public string StudentId { get; set; }
        [DataMember] public string TutorId { get; set; }
        [DataMember] public string Comment { get; set; }
        [DataMember] public char AttendanceStatus { get; set; }
        [DataMember] public string Reason { get; set; }
        [DataMember] public int Rating { get; set; }    
    }
}
