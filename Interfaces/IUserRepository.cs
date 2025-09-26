using ArqPay.Models;

namespace ArqPay.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserById(Guid id);
        Task<User?> GetUserByEmail(string email);
        Task<IEnumerable<User>> GetAllUsers();
        Task<Guid> CreateUser(User user);
        Task<int> UpdateUser(User user);
        Task<int> DeleteUser(Guid id);

        Task<User?> GetUserWithCredentials(string email);
    }
}
