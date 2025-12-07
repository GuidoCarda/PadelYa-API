using padelya_api.DTOs.Tournament;
using System.Threading.Tasks;

namespace padelya_api.Services
{
    public interface IMatchResultService
    {
        Task<MatchResultResponseDto> RegisterMatchResultAsync(RegisterMatchResultDto resultDto);
        Task<bool> UpdateMatchResultAsync(int matchId, RegisterMatchResultDto resultDto);
    }
}

