using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTOs
{
    [DataContract]
    public class RegisterStudentRequest
    {
        [DataMember] public string StudentId { get; set; }
        [DataMember] public string FirstName { get; set; }
        [DataMember] public string PaternalSurname { get; set; }
        [DataMember] public string MaternalSurname { get; set; }
        [DataMember] public int CareerId { get; set; }
        [DataMember] public string Email { get; set; }
        [DataMember] public string Password { get; set; }
        [DataMember] public string TutorId { get; set; }  
    }
}
