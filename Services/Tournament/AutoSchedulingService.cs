using Microsoft.EntityFrameworkCore;
using padelya_api.Constants;
using padelya_api.Data;
using padelya_api.DTOs.Tournament;
using padelya_api.Models;
using padelya_api.Models.Tournament;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace padelya_api.Services
{
    public class AutoSchedulingService : IAutoSchedulingService
    {
        private readonly PadelYaDbContext _context;

        public AutoSchedulingService(PadelYaDbContext context)
        {
            _context = context;
        }

        public async Task<AutoSchedulingResultDto> AutoScheduleMatchesAsync(
            List<int> matchIds,
            int tournamentId)
        {
            var result = new AutoSchedulingResultDto
            {
                TournamentId = tournamentId
            };

            var tournament = await _context.Tournaments
                .Include(t => t.TournamentPhases)
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            if (tournament == null)
            {
                result.Message = "Torneo no encontrado";
                return result;
            }

            var availableCourts = await _context.Courts
                .Where(c => c.CourtStatus == CourtStatus.Available)
                .ToListAsync();

            if (!availableCourts.Any())
            {
                result.Message = "No hay canchas disponibles para programar";
                return result;
            }

            var courtIds = availableCourts.Select(c => c.Id).ToList();

            // Obtener los partidos a programar
            var matches = await _context.TournamentMatches
                .Include(m => m.Bracket)
                    .ThenInclude(b => b.Phase)
                .Where(m => matchIds.Contains(m.Id))
                .ToListAsync();

            // Agrupar partidos por fase
            var matchesByPhase = matches.GroupBy(m => m.Bracket.PhaseId).ToList();

            foreach (var phaseGroup in matchesByPhase)
            {
                var phase = phaseGroup.First().Bracket.Phase;
                var phaseMatches = phaseGroup.ToList();

                foreach (var match in phaseMatches)
                {
                    try
                    {
                        if (!match.CoupleOneId.HasValue || !match.CoupleTwoId.HasValue)
                        {
                            result.FailedMatches.Add(new FailedMatchInfo
                            {
                                MatchId = match.Id,
                                Reason = "El partido no tiene ambas parejas asignadas"
                            });
                            result.TotalMatchesFailed++;
                            continue;
                        }

                        var availableSlot = await FindNextAvailableSlotAsync(
                            tournamentId,
                            phase.StartDate,
                            phase.EndDate,
                            courtIds,
                            90 
                        );

                        if (availableSlot == null)
                        {
                            result.FailedMatches.Add(new FailedMatchInfo
                            {
                                MatchId = match.Id,
                                Reason = "No se encontró un slot disponible en el rango de fechas de la fase"
                            });
                            result.TotalMatchesFailed++;
                            continue;
                        }

                        var courtSlot = new CourtSlot
                        {
                            CourtId = availableSlot.CourtId,
                            Date = availableSlot.Date,
                            StartTime = TimeOnly.FromTimeSpan(availableSlot.StartTime),
                            EndTime = TimeOnly.FromTimeSpan(availableSlot.EndTime),
                            Status = CourtSlotStatus.Active
                        };

                        _context.CourtSlots.Add(courtSlot);
                        await _context.SaveChangesAsync();

                        match.CourtSlotId = courtSlot.Id;
                        match.TournamentMatchState = "Programado";
                        await _context.SaveChangesAsync();

                        result.ScheduledMatches.Add(new ScheduledMatchInfo
                        {
                            MatchId = match.Id,
                            CourtId = availableSlot.CourtId,
                            CourtName = availableSlot.CourtName,
                            ScheduledDate = availableSlot.Date,
                            StartTime = availableSlot.StartTime,
                            EndTime = availableSlot.EndTime
                        });
                        result.TotalMatchesScheduled++;
                    }
                    catch (Exception ex)
                    {
                        result.FailedMatches.Add(new FailedMatchInfo
                        {
                            MatchId = match.Id,
                            Reason = $"Error al programar: {ex.Message}"
                        });
                        result.TotalMatchesFailed++;
                    }
                }
            }

            result.Message = result.HasConflicts
                ? $"Programación completada con conflictos: {result.TotalMatchesScheduled} exitosos, {result.TotalMatchesFailed} fallidos"
                : $"Programación completada exitosamente: {result.TotalMatchesScheduled} partidos programados";

            return result;
        }

        public async Task<AvailableSlotDto?> FindNextAvailableSlotAsync(
            int tournamentId,
            DateTime phaseStartDate,
            DateTime phaseEndDate,
            List<int> courtIds,
            int durationMinutes = 90)
        {
            var searchStartTime = new TimeSpan(8, 0, 0);
            var searchEndTime = new TimeSpan(23, 0, 0);
            var matchDuration = TimeSpan.FromMinutes(durationMinutes);

            for (var date = phaseStartDate.Date; date <= phaseEndDate.Date; date = date.AddDays(1))
            {
                if (date < DateTime.Now.Date)
                {
                    continue;
                }

                foreach (var courtId in courtIds)
                {
                    var court = await _context.Courts.FindAsync(courtId);
                    if (court == null) continue;

                    var startTime = court.OpeningTime.ToTimeSpan() > searchStartTime 
                        ? court.OpeningTime.ToTimeSpan() 
                        : searchStartTime;
                    
                    var endTime = court.ClosingTime.ToTimeSpan() < searchEndTime 
                        ? court.ClosingTime.ToTimeSpan() 
                        : searchEndTime;

                    for (var currentTime = startTime; 
                         currentTime.Add(matchDuration) <= endTime; 
                         currentTime = currentTime.Add(matchDuration))
                    {
                        var slotEndTime = currentTime.Add(matchDuration);

                        bool isAvailable = await IsSlotAvailableAsync(
                            courtId, 
                            date, 
                            currentTime, 
                            slotEndTime
                        );

                        if (isAvailable)
                        {
                            return new AvailableSlotDto
                            {
                                CourtId = courtId,
                                CourtName = court.Name,
                                Date = date,
                                StartTime = currentTime,
                                EndTime = slotEndTime
                            };
                        }
                    }
                }
            }

            return null;
        }

        private async Task<bool> IsSlotAvailableAsync(
            int courtId,
            DateTime date,
            TimeSpan startTime,
            TimeSpan endTime)
        {
            var startTimeOnly = TimeOnly.FromTimeSpan(startTime);
            var endTimeOnly = TimeOnly.FromTimeSpan(endTime);

            var conflictingSlots = await _context.CourtSlots
                .Include(cs => cs.Booking)
                .Where(cs =>
                    cs.CourtId == courtId &&
                    cs.Date.Date == date.Date &&
                    cs.Status == CourtSlotStatus.Active &&
                    (
                        (startTimeOnly >= cs.StartTime && startTimeOnly < cs.EndTime) ||
                        (endTimeOnly > cs.StartTime && endTimeOnly <= cs.EndTime) ||
                        (startTimeOnly <= cs.StartTime && endTimeOnly >= cs.EndTime)
                    )
                )
                .ToListAsync();

            foreach (var slot in conflictingSlots)
            {
                if (slot.Booking != null)
                {
                    var booking = slot.Booking;
                    if (booking.Status == BookingStatus.ReservedPaid || 
                        booking.Status == BookingStatus.ReservedDeposit)
                    {
                        return false;
                    }
                }

                if (slot.Lesson != null)
                {
                    return false;
                }

                if (slot.TournamentMatch != null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

