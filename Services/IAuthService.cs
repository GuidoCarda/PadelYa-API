using padelya_api.DTOs.Auth;
using padelya_api.DTOs.User;
using padelya_api.Models;

namespace padelya_api.Services
{
    public interface IAuthService
    {
        //Task<User?> RegisterAsync(UserDto request);
        Task<User?> RegisterPlayerAsync(RegisterPlayerDto request);
        Task<User?> RegisterTeacherAsync(RegisterTeacherDto request);
        Task<TokenResponseDto?> LoginAsync(UserLoginDto request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
        Task<bool> RecoverPasswordAsync(string email);
 
    }
}
