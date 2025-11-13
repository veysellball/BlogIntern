using BlogIntern.Services.Interfaces;

namespace BlogIntern.Services.Implements
{
    public static class LoginFactory
    {
        public static ILoginStrategy Create(string? loginType)
        {
            return new EmailLoginStrategy();
        }
    }
}
