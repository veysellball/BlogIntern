using BlogIntern.Dtos;
using BlogIntern.Services.Implements;
using BlogIntern.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogIntern.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(ILoginService loginService, JwtTokenService jwtTokenService)
        {
            _loginService = loginService;
            _jwtTokenService = jwtTokenService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _loginService.LoginAsync(request);

            if (result == null)
                return Unauthorized(new { success = false, message = "Email veya şifre hatalı" });

            return Ok(new
            {
                success = true,
                message = "Giriş başarılı",
                accessToken = result.AccessToken,
                accessTokenExpiresAt = result.AccessTokenExpiresAt,
                refreshToken = result.RefreshToken,
                refreshTokenExpiresAt = result.RefreshTokenExpiresAt
            });
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            var refreshed = await _loginService.RefreshAsync(request.RefreshToken);

            if (refreshed == null)
                return Unauthorized(new { success = false, message = "Refresh token geçersiz veya süresi dolmuş." });

            return Ok(new
            {
                success = true,
                message = "Token yenilendi.",
                accessToken = refreshed.AccessToken,
                accessTokenExpiresAt = refreshed.AccessTokenExpiresAt,
                refreshToken = refreshed.RefreshToken,
                refreshTokenExpiresAt = refreshed.RefreshTokenExpiresAt
            });
        }

        [HttpPost("decode")]
        public IActionResult Decode()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(token))
                return BadRequest("Token header içinde gönderilmedi.");

            token = token.Replace("Bearer ", "");

            var decoded = _jwtTokenService.DecodeToken(token);

            return Ok(decoded);
        }
    }
}
