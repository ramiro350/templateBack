// Interfaces/IUserService.cs
using ArqPay.Models;
using ArqPay.Models.Requests;

namespace ArqPay.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUser(GetUserRequest getUserRequest);
        Task<User?> GetUserById(Guid id);
        Task<IEnumerable<User>> GetAllUsers();
        Task<Guid> CreateUser(CreateUserRequest createUserRequest);
        void UpdateUser(UpdateUserRequest request);
        Task<int> DeleteUser(Guid id);
        Task<User?> GetUserWithCredentials(string email);
    }
}
