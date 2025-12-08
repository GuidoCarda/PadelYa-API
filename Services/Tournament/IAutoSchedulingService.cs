using padelya_api.DTOs.Tournament;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace padelya_api.Services
{
    public interface IAutoSchedulingService
    {
        Task<AutoSchedulingResultDto> AutoScheduleMatchesAsync(
            List<int> matchIds, 
            int tournamentId);

        Task<AvailableSlotDto?> FindNextAvailableSlotAsync(
            int tournamentId,
            DateTime phaseStartDate,
            DateTime phaseEndDate,
            List<int> courtIds,
            int durationMinutes = 90);
    }
}

