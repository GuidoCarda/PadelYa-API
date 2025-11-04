using padelya_api.DTOs.Tournament;
using System.Threading.Tasks;

namespace padelya_api.Services
{
    public interface IMatchSchedulingService
    {
        Task<MatchScheduleResponseDto> AssignMatchScheduleAsync(AssignMatchScheduleDto scheduleDto);
        Task<bool> UnassignMatchScheduleAsync(int matchId);
    }
}

