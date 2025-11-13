using BlogIntern.Dtos;
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

        public AuthController(ILoginService loginService)
        {
            _loginService = loginService;
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
    }
}
