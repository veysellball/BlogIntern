using System.Threading.Tasks;

namespace BlogIntern.Services.Interfaces
{
    public interface ILoginService
    {
        Task<bool> LoginAsync(string email, string password);
    }
}
