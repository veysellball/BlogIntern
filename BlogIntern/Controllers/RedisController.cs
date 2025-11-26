using BlogIntern.Services.Implements;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace BlogIntern.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class RedisController : ControllerBase
    {
        private readonly RedisTestService _redis;

        public RedisController(RedisTestService redis)
        {
            _redis = redis;
        }

        [HttpGet("db")]
        public async Task<IActionResult> FromDb()
        {
            var sw = Stopwatch.StartNew();
            var data = await _redis.GetFromDb();
            sw.Stop();

            return Ok(new { source = "db", time = sw.ElapsedMilliseconds, count = data.Count });
        }

        [HttpGet("redis")]
        public async Task<IActionResult> FromRedis()
        {
            var sw = Stopwatch.StartNew();
            var data = await _redis.GetFromRedis();
            sw.Stop();

            return Ok(new { source = "redis", time = sw.ElapsedMilliseconds, count = data.Count });
        }

        [HttpGet("clear")]
        public async Task<IActionResult> Clear()
        {
            await _redis.ClearCache();
            return Ok(new { message = "cache cleared" });
        }
    }


}