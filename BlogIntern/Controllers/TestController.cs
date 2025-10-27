
using BlogIntern.Data;
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

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                // Basit bir veritabanı bağlantı testi
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
    }
}
