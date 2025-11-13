using BlogIntern.Data;
using BlogIntern.Models;
using BlogIntern.Dtos;
using BlogIntern.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogIntern.Services.Implements
{
    public class EmailLoginStrategy : ILoginStrategy
    {
        public async Task<User?> LoginAsync(LoginRequestDto dto, AppDbContext context)
        {
            
           
            var normalizedEmail = dto.Email.Trim().ToLower();

            var user = await context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

            if (user == null)
            {
                
                return null;
            }

    
            bool verified = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
         

            if (!verified)
            {
              
                return null;
            }

          
            return user;
        }
    }
}
