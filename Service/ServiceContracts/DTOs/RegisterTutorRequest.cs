using System.Runtime.Serialization;

namespace ServiceContracts.DTOs
{
    [DataContract]
    public class RegisterTutorRequest
    {
        [DataMember] public string TutorId { get; set; }
        [DataMember] public string FirstName { get; set; }
        [DataMember] public string PaternalSurname { get; set; }
        [DataMember] public string MaternalSurname { get; set; }
        [DataMember] public string Phone { get; set; }
        [DataMember] public string Email { get; set; }
        [DataMember] public string Password { get; set; }
    }
}
