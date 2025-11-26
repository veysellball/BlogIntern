using BlogIntern.Models; 
using BlogIntern.Dtos;  
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BlogIntern.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetUserById(int id);
        Task<User> AddNewUser(UserCreateDto dto);
        Task<bool> DeleteUserById(int id);
        Task<bool> SoftDeleteUserById(int id);
        Task<bool> ReActivateUserById(int id);
        Task<List<User>> GetAllUsersOrderByDate();
        Task<List<UserWithRoleDto>> GetUsersWithRoles_SP();
        Task<List<UserWithRoleSpDto>> GetUsersByRole_SP(string roleName);




    }
}
