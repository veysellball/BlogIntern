using System;

namespace BlogIntern.Models
{
    public class ApiLog
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? Path { get; set; }
        public string? Method { get; set; }
        public string? QueryString { get; set; }
        public string? RequestBody { get; set; }
        public int StatusCode { get; set; }
        public string? ResponseBody { get; set; }
        public long DurationMs { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        //stacktrace        public string? StackTrace { get; set; }
    }
}
