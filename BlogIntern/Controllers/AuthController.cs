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
        private readonly LoginService _loginService;
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(LoginService loginService, JwtTokenService jwtTokenService)
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
                token = result.Token,
                expiration = result.Expiration
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

