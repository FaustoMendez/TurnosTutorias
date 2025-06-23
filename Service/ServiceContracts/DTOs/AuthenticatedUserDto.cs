using System.Runtime.Serialization;

namespace ServiceContracts.DTOs
{
    [DataContract]
    public class AuthenticatedUserDto
    {
        [DataMember] public string Id { get; set; }
        [DataMember] public string FirstName { get; set; }
        [DataMember] public string PaternalSurname { get; set; }
        [DataMember] public string MaternalSurname { get; set; }
        [DataMember] public string Email { get; set; }
        [DataMember] public int MajorId { get; set; }   
        [DataMember] public string Role { get; set; }   
    }
}
