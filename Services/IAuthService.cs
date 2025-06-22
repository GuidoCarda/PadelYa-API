using padelya_api.DTOs.Auth;
using padelya_api.DTOs.User;

namespace padelya_api.Services
{
    public interface IAuthService
    {
        Task<TokenResponseDto?> RegisterPlayerAsync(RegisterPlayerDto request);
        Task<TokenResponseDto?> RegisterTeacherAsync(RegisterTeacherDto request);
        Task<TokenResponseDto?> LoginAsync(UserLoginDto request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
        Task<bool> RecoverPasswordAsync(string email);

    }
}
