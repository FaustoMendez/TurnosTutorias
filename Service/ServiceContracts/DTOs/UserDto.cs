using System.Runtime.Serialization;

namespace ServiceContracts.DTOs
{
    [DataContract]
    public class UserDto
    {
        [DataMember] public string UserId { get; set; }
        [DataMember] public string FirstName { get; set; }
        [DataMember] public string PaternalSurname { get; set; }
        [DataMember] public string MaternalSurname { get; set; }
        [DataMember] public string Email { get; set; }
        [DataMember] public string MajorId { get; set; }
    }
}
