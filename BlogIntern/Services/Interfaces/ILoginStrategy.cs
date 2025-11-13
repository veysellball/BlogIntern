using BlogIntern.Data;
using BlogIntern.Dtos;
using BlogIntern.Models;

namespace BlogIntern.Services.Interfaces
{
    public interface ILoginStrategy
    {
        Task<User?> LoginAsync(LoginRequestDto dto, AppDbContext context);
    }
}