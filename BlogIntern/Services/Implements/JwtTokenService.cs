using BlogIntern.Dtos;
using BlogIntern.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BlogIntern.Services.Implements
{
    public class JwtTokenService
    {
        private readonly string _secret;
        private readonly string _issuer;

        public JwtTokenService(IConfiguration config)
        {
            _secret = config["Jwt:Secret"]!;
            _issuer = config["Jwt:Issuer"]!;
        }

       
        public TokenResponseDto GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("userId", user.Id.ToString())
            };

            
            if (user.UserRoles != null)
            {
                claims.AddRange(
                    user.UserRoles.Select(r => new Claim(ClaimTypes.Role, r.Role.Name))
                );
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            
            var expires = DateTime.UtcNow.AddMinutes(15);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _issuer,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new TokenResponseDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                AccessTokenExpiresAt = expires
            };
        }

        
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

   
        public TokenDecodeDto DecodeToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var email = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            var roles = jwt.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var expiration = jwt.ValidTo;

            return new TokenDecodeDto
            {
                Email = email,
                Roles = roles,
                Expiration = expiration
            };
        }
    }
}
