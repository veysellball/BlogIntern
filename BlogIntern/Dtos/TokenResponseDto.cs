namespace BlogIntern.Dtos
{
    public class TokenResponseDto
    {
        public string Token { get; set; } = null!;
        public DateTime Expiration { get; set; }
    }
}
