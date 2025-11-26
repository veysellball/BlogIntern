using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AsyncDemoController : ControllerBase
{
    private readonly ILogger<AsyncDemoController> _logger;

    public AsyncDemoController(ILogger<AsyncDemoController> logger)
    {
        _logger = logger;
    }

    [HttpGet("sync")]
    public IActionResult Sync()
    {
        _logger.LogInformation("SYNC başladı");
        Thread.Sleep(5000);
        _logger.LogInformation("SYNC bitti");
        return Ok("Senkron");
    }

    [HttpGet("async")]
    public async Task<IActionResult> Async()
    {
        _logger.LogInformation("ASYNC başladı");
        await Task.Delay(5000);
        _logger.LogInformation("ASYNC bitti");
        return Ok("Asenkron");
    }
}
