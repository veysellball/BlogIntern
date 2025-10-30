using BlogIntern.Models; // User entity’nin bulunduğu namespace
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BlogIntern.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetUserById(int id);
        Task<User> AddNewUser(User user);
        Task<bool> DeleteUserById(int id);
        Task<bool> SoftDeleteUserById(int id);
        Task<bool> ReActivateUserById(int id);

    }
}
