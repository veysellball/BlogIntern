using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlogIntern.Dtos
{
    public class LoginRequestDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

        [JsonIgnore]  // Swagger ve dış JSON isteğinde görünmez
        public string LoginType { get; set; } = "email";
    }
}
