using BlogIntern.Dtos;
using System.Threading.Tasks;

namespace BlogIntern.Services.Interfaces
{
    public interface ILoginService
    {
        Task<TokenResponseDto?> LoginAsync(LoginRequestDto dto);

        Task<TokenResponseDto?> RefreshAsync(string refreshToken);
    }
}
