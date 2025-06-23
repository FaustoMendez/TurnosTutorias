using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTOs
{
    [DataContract]
    public class LoginRequest
    {
        [DataMember] public string StudentId { get; set; }
        [DataMember] public string Password { get; set; }
    }
}
