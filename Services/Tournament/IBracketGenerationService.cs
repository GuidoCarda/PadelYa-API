using padelya_api.DTOs.Tournament;
using System.Threading.Tasks;

namespace padelya_api.Services
{
    public interface IBracketGenerationService
    {
        Task<GenerateBracketResponseDto?> GenerateTournamentBracketAsync(int tournamentId);
        Task<GenerateBracketResponseDto?> GenerateTournamentBracketAsync(int tournamentId, bool autoSchedule);
        Task<TournamentPhaseWithBracketsDto?> GetTournamentBracketAsync(int tournamentId);
        Task<List<TournamentPhaseWithBracketsDto>> GetAllTournamentPhasesAsync(int tournamentId);
    }
}

