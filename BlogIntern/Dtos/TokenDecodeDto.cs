namespace BlogIntern.Dtos
{
    public class TokenDecodeDto
    {
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public DateTime Expiration { get; set; }
    }
}
