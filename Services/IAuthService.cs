using padelya_api.DTOs;
using padelya_api.Models;

namespace padelya_api.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<User?> RegisterPlayerAsync(PlayerRegisterDto request);
        Task<User?> RegisterTeacherAsync(TeacherRegisterDto request);
        Task<TokenResponseDto?> LoginAsync(UserDto request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
    }
}
