using StudentAPI.DTOs;
using static StudentAPI.DTOs.AuthDtos;
namespace StudentAPI.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDto> LoginAsync(LoginDto dto);
         Task<TokenResponseDto> RegisterAsync(RegisterDto dto);
    }
}
