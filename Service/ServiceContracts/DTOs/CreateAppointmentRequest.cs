using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTOs
{
    [DataContract]
    public class CreateAppointmentRequest
    {
        [DataMember] public string StudentId { get; set; }
        [DataMember] public string TutorId { get; set; }
        [DataMember] public int SessionId { get; set; }
    }
}
