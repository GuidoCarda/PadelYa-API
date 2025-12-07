using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Tournament;
using padelya_api.Models;
using padelya_api.Models.Tournament;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace padelya_api.Services
{
    public class MatchSchedulingService(PadelYaDbContext context) : IMatchSchedulingService
    {
        private readonly PadelYaDbContext _context = context;

        public async Task<MatchScheduleResponseDto> AssignMatchScheduleAsync(AssignMatchScheduleDto scheduleDto)
        {
            // 1. Validar que el partido existe y cargar el torneo
            var match = await _context.TournamentMatches
                .Include(m => m.Bracket)
                .ThenInclude(b => b.Phase)
                .ThenInclude(p => p.Tournament)
                .FirstOrDefaultAsync(m => m.Id == scheduleDto.MatchId);

            if (match == null)
            {
                throw new ArgumentException($"No se encontró el partido con ID {scheduleDto.MatchId}");
            }

            // 2. Validar que la cancha existe
            var court = await _context.Courts.FindAsync(scheduleDto.CourtId);
            if (court == null)
            {
                throw new ArgumentException($"No se encontró la cancha con ID {scheduleDto.CourtId}");
            }

            // 3. Calcular hora de finalización
            var endTime = scheduleDto.StartTime.Add(TimeSpan.FromMinutes(scheduleDto.DurationMinutes));

            // 4. Validar que la fecha/hora esté dentro del rango del torneo completo
            var tournament = match.Bracket.Phase.Tournament;
            var tournamentStartDate = tournament.TournamentStartDate.Date;
            var tournamentEndDate = tournament.TournamentEndDate.Date;
            var scheduledDate = scheduleDto.ScheduledDate.Date;

            if (scheduledDate < tournamentStartDate || scheduledDate > tournamentEndDate)
            {
                throw new ArgumentException(
                    $"La fecha programada debe estar dentro del rango del torneo ({tournamentStartDate:dd/MM/yyyy} - {tournamentEndDate:dd/MM/yyyy})"
                );
            }

            // 5. Validar disponibilidad de la cancha en ese horario
            var startTimeOnly = TimeOnly.FromTimeSpan(scheduleDto.StartTime);
            var endTimeOnly = TimeOnly.FromTimeSpan(endTime);

            var conflictingSlot = await _context.CourtSlots
                .Where(cs =>
                    cs.CourtId == scheduleDto.CourtId &&
                    cs.Date.Date == scheduledDate &&
                    cs.Status == CourtSlotStatus.Active &&
                    (
                        // El nuevo slot comienza durante un slot existente
                        (startTimeOnly >= cs.StartTime && startTimeOnly < cs.EndTime) ||
                        // El nuevo slot termina durante un slot existente
                        (endTimeOnly > cs.StartTime && endTimeOnly <= cs.EndTime) ||
                        // El nuevo slot contiene completamente un slot existente
                        (startTimeOnly <= cs.StartTime && endTimeOnly >= cs.EndTime)
                    )
                )
                .FirstOrDefaultAsync();

            if (conflictingSlot != null)
            {
                throw new ArgumentException(
                    $"La cancha {court.Name} no está disponible en el horario seleccionado. " +
                    $"Ya existe una reserva de {conflictingSlot.StartTime:HH\\:mm} a {conflictingSlot.EndTime:HH\\:mm}."
                );
            }

            // 6. Si el partido ya tenía un CourtSlot asignado, eliminarlo
            if (match.CourtSlotId.HasValue)
            {
                var existingSlot = await _context.CourtSlots.FindAsync(match.CourtSlotId.Value);
                if (existingSlot != null)
                {
                    _context.CourtSlots.Remove(existingSlot);
                }
            }

            // 7. Crear el nuevo CourtSlot
            var courtSlot = new CourtSlot
            {
                CourtId = scheduleDto.CourtId,
                Date = scheduledDate,
                StartTime = startTimeOnly,
                EndTime = endTimeOnly,
                Status = CourtSlotStatus.Active
            };

            _context.CourtSlots.Add(courtSlot);
            await _context.SaveChangesAsync();

            // 8. Asignar el CourtSlot al partido
            match.CourtSlotId = courtSlot.Id;
            match.TournamentMatchState = "Programado";
            await _context.SaveChangesAsync();

            // 9. Retornar respuesta
            return new MatchScheduleResponseDto
            {
                MatchId = match.Id,
                ScheduledDate = scheduledDate,
                StartTime = scheduleDto.StartTime,
                EndTime = endTime,
                CourtId = court.Id,
                CourtName = court.Name,
                Message = "Partido programado exitosamente"
            };
        }

        public async Task<bool> UnassignMatchScheduleAsync(int matchId)
        {
            var match = await _context.TournamentMatches
                .Include(m => m.CourtSlot)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null)
            {
                return false;
            }

            // Si tiene un CourtSlot asignado, eliminarlo
            if (match.CourtSlotId.HasValue && match.CourtSlot != null)
            {
                _context.CourtSlots.Remove(match.CourtSlot);
                match.CourtSlotId = null;
                match.TournamentMatchState = "Pendiente";
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}

