using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using ServiceContracts.DTOs;

namespace ServiceContracts
{
    [DataContract]
    public class DuplicateStudentFault
    {
        [DataMember] public string Message { get; set; }
    }

    [DataContract]
    public class DuplicateTutorFault
    {
        [DataMember] public string Message { get; set; }
    }

    [DataContract]
    public class CapacityFault
    {
        [DataMember] public string Message { get; set; }
    }

    [ServiceContract]
    public interface IUserService
    {
        [OperationContract]
        [FaultContract(typeof(DuplicateTutorFault))] 
        void RegisterTutor(RegisterTutorRequest request);

        [OperationContract]
        [FaultContract(typeof(DuplicateStudentFault))]
        [FaultContract(typeof(CapacityFault))]
        void RegisterStudent(RegisterStudentRequest request);

        [OperationContract]
        AuthenticatedUserDto LoginUser(LoginRequest request);

        [OperationContract]
        List<TutorDto> GetAvailableTutors();

        [OperationContract]
        List<UserDto> GetAllUsers();

        [OperationContract]
        List<TutorDto> GetAllTutors();

        [OperationContract]
        List<CareerDto> GetAllCareers();
    }
}
