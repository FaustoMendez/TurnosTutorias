using System.Runtime.Serialization;

namespace ServiceContracts.DTOs
{
    [DataContract]
    public class CareerDto
    {
        [DataMember] public int CareerId { get; set; }
        [DataMember] public string CareerName { get; set; }

        public string FullName => CareerName;
        public override string ToString() => FullName;
    }
}
