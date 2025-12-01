using BlogIntern.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogIntern.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestController(AppDbContext context)
        {
            _context = context;
        }

        
        [AllowAnonymous]
        [HttpGet("public")]
        public IActionResult PublicTest()
        {
            return Ok("🌍 Public endpoint: Bu endpoint'e herkes erişebilir.");
        }

        
        [Authorize]
        [HttpGet("secure")]
        public IActionResult SecureTest()
        {
            return Ok("🔐 Secure endpoint: JWT token ile giriş yapılmış!");
        }

      
        [AllowAnonymous]
        [HttpGet("db-check")]
        public IActionResult CheckDatabase()
        {
            try
            {
                var canConnect = _context.Database.CanConnect();
                if (canConnect)
                    return Ok("✅ Veritabanı bağlantısı başarılı!");
                else
                    return StatusCode(500, "❌ Veritabanına bağlanılamadı.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ Hata: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("cause-error")]
        public IActionResult CauseError()
        {
            throw new Exception("TEST_EXCEPTION_FROM_TEST_CONTROLLER");
        }

    }
}
