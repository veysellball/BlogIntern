using BlogIntern.Data;
using BlogIntern.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace BlogIntern.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public RequestResponseLoggingMiddleware(RequestDelegate next,
                                                ILogger<RequestResponseLoggingMiddleware> logger,
                                                IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            using var scope = _scopeFactory.CreateScope();
            var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var userId = context.User?.FindFirst("userId")?.Value ?? "anonymous";

            var request = context.Request;
            request.EnableBuffering();

            string requestBody = "";
            if (request.Body.CanRead)
            {
                request.Body.Position = 0;
                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                request.Body.Position = 0;
            }

            var originalBody = context.Response.Body;
            var newBody = new MemoryStream();
            context.Response.Body = newBody;

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                var log = new ApiLog
                {
                    UserId = userId,
                    Path = request.Path,
                    Method = request.Method,
                    QueryString = request.QueryString.ToString(),
                    RequestBody = requestBody,
                    StatusCode = 500,
                    ResponseBody = ex.Message,
                    DurationMs = stopwatch.ElapsedMilliseconds,
                    ErrorMessage = ex.ToString()
                };

                _db.ApiLogs.Add(log);
                await _db.SaveChangesAsync();

                throw;
            }

            stopwatch.Stop();

            newBody.Position = 0;
            var responseBody = await new StreamReader(newBody).ReadToEndAsync();
            newBody.Position = 0;

            var logEntry = new ApiLog
            {
                UserId = userId,
                Path = request.Path,
                Method = request.Method,
                QueryString = request.QueryString.ToString(),
                RequestBody = requestBody,
                StatusCode = context.Response.StatusCode,
                ResponseBody = responseBody,
                DurationMs = stopwatch.ElapsedMilliseconds
            };

            _db.ApiLogs.Add(logEntry);
            await _db.SaveChangesAsync();

            await newBody.CopyToAsync(originalBody);
            context.Response.Body = originalBody;
        }
    }
}
