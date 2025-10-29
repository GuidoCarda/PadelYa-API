using padelya_api.DTOs.Tournament;
using System.Threading.Tasks;

namespace padelya_api.Services
{
    public interface IBracketGenerationService
    {
        Task<GenerateBracketResponseDto?> GenerateTournamentBracketAsync(int tournamentId);
        Task<TournamentPhaseWithBracketsDto?> GetTournamentBracketAsync(int tournamentId);
    }
}

