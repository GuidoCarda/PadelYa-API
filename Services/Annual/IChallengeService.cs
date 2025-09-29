using System.Collections.Generic;
using System.Threading.Tasks;
using padelya_api.DTOs.Challenge;
using padelya_api.Models.Challenge;

namespace padelya_api.Services.Annual
{
    public interface IChallengeService
    {
        Task<Challenge> CreateAsync(int year, CreateChallengeDto dto);
        Task<Challenge> RespondAsync(int id, bool accept);
        Task<Challenge> RegisterResultAsync(int id, RegisterChallengeResultDto dto);
        Task<List<Challenge>> GetHistoryAsync(int playerId, int? year = null);
        Task<Challenge> ValidateAsync(int id, RegisterChallengeResultDto dto);
    }
}

