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
        [DataMember] public string FirstName { get; set; }
        [DataMember] public string PaternalSurname { get; set; }
        [DataMember] public string MaternalSurname { get; set; }
        [DataMember] public string Phone { get; set; }
        [DataMember] public string Email { get; set; }
        [DataMember] public int CurrentLoad { get; set; }
        public string FullName => $"{FirstName} {PaternalSurname}";
        public override string ToString() => FullName;
    }
}
