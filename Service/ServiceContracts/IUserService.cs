using System.Collections.Generic;
using System.ServiceModel;
using ServiceContracts.DTOs;

namespace ServiceContracts
{
    [ServiceContract]
    public interface IUserService
    {
        [OperationContract]
        void RegisterTutor(RegisterTutorRequest request);

        [OperationContract]
        void RegisterStudent(RegisterStudentRequest request);

        [OperationContract]
        AuthenticatedUserDto LoginUser(LoginRequest request);

        [OperationContract]
        List<TutorDto> GetAvailableTutors();

        [OperationContract]
        List<UserDto> GetAllUsers();
    }
}
