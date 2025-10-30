using BlogIntern.Data;
using BlogIntern.Models;
using BlogIntern.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace BlogIntern.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users
                .Where(u => u.IsActive) // Sadece aktif kullanıcılar
                .ToListAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
            if (user == null)
                throw new Exception($"User with id {id} not found or inactive.");

            return user;
        }

        public async Task<User> AddNewUser(User user)
        {
            user.IsActive = true;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            user.IsActive = false;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReActivateUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            user.IsActive = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
