using BlogIntern.Data;
using BlogIntern.Dtos;
using BlogIntern.Models;
using BlogIntern.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BlogIntern.Services.Implements
{
    public class LoginService : ILoginService
    {
        private readonly AppDbContext _context;
        private readonly JwtTokenService _jwt;

        public LoginService(AppDbContext context, JwtTokenService jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        public async Task<TokenResponseDto?> LoginAsync(LoginRequestDto dto)
        {
          

            var strategy = LoginFactory.Create(dto.LoginType);
            

            var user = await strategy.LoginAsync(dto, _context);

            if (user == null)
            {
                return null;
            }

           
            var token = _jwt.GenerateToken(user);
          

            return token;
        }
    }
}
