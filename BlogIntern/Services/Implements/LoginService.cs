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
                return null;

           
            await _context.Entry(user)
                .Collection(u => u.UserRoles)
                .Query()
                .Include(ur => ur.Role)
                .LoadAsync();

            
            var accessTokenDto = _jwt.GenerateAccessToken(user);

            
            var refreshTokenValue = _jwt.GenerateRefreshToken();
            var refreshTokenExpires = DateTime.UtcNow.AddDays(7);

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenValue,
                ExpiresAt = refreshTokenExpires
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new TokenResponseDto
            {
                AccessToken = accessTokenDto.AccessToken,
                AccessTokenExpiresAt = accessTokenDto.AccessTokenExpiresAt,
                RefreshToken = refreshTokenValue,
                RefreshTokenExpiresAt = refreshTokenExpires
            };
        }

       
        public async Task<TokenResponseDto?> RefreshAsync(string refreshToken)
        {
            var tokenEntity = await _context.RefreshTokens
                .Include(rt => rt.User)
                    .ThenInclude(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        
            if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.IsExpired)
                return null;

            var user = tokenEntity.User;

            
            tokenEntity.RevokedAt = DateTime.UtcNow;

           
            var accessTokenDto = _jwt.GenerateAccessToken(user);

          
            var newRefreshValue = _jwt.GenerateRefreshToken();
            var newRefreshExpires = DateTime.UtcNow.AddDays(7);

            var newRefreshEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshValue,
                ExpiresAt = newRefreshExpires
            };

            _context.RefreshTokens.Add(newRefreshEntity);

            await _context.SaveChangesAsync();

            return new TokenResponseDto
            {
                AccessToken = accessTokenDto.AccessToken,
                AccessTokenExpiresAt = accessTokenDto.AccessTokenExpiresAt,
                RefreshToken = newRefreshValue,
                RefreshTokenExpiresAt = newRefreshExpires
            };
        }
    }
}
