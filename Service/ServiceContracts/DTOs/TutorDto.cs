using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTOs
{
    [DataContract]
    public class TutorDto
    {
        [DataMember] public string TutorId { get; set; }
        [DataMember] public string FullName { get; set; }
        [DataMember] public int CurrentLoad { get; set; } 
    }
}
