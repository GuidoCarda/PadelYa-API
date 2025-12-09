using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.DTOs.Tournament;
using padelya_api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using padelya_api.Attributes;
using padelya_api.Constants;

namespace padelya_api.Controllers
{
    [ApiController]
    [Route("api/Tournament")]
    public class TournamentController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;
        private readonly IBracketGenerationService _bracketGenerationService;
        private readonly IMatchSchedulingService _matchSchedulingService;
        private readonly IMatchResultService _matchResultService;
        private readonly IAutoSchedulingService _autoSchedulingService;
        private readonly PadelYaDbContext _context;

        public TournamentController(
            ITournamentService tournamentService,
            IBracketGenerationService bracketGenerationService,
            IMatchSchedulingService matchSchedulingService,
            IMatchResultService matchResultService,
            IAutoSchedulingService autoSchedulingService,
            PadelYaDbContext context)
        {
            _tournamentService = tournamentService;
            _bracketGenerationService = bracketGenerationService;
            _matchSchedulingService = matchSchedulingService;
            _matchResultService = matchResultService;
            _autoSchedulingService = autoSchedulingService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTournament([FromBody] CreateTournamentDto createTournamentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newTournament = await _tournamentService.CreateTournamentAsync(createTournamentDto);
                if (newTournament == null)
                {
                    return BadRequest("No se pudo crear el torneo.");
                }
                return StatusCode(201, newTournament);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTournaments()
        {
            try
            {
                var tournaments = await _tournamentService.GetTournamentsAsync();
                return Ok(tournaments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
        [HttpDelete("{id}")]
        [RequirePermission(Permissions.Tournament.Delete)] 
        public async Task<IActionResult> DeleteTournament(int id)
        {
            try
            {
                var result = await _tournamentService.DeleteTournamentAsync(id);

                if (!result)
                {
                    return NotFound($"No se encontró el torneo con ID {id}.");
                }

                return Ok($"Torneo con ID {id} eliminado correctamente.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateTournament(int id, [FromBody] UpdateTournamentDto updateDto)
        {
            try
            {
                var updatedTournament = await _tournamentService.UpdateTournamentAsync(id, updateDto);

                if (updatedTournament == null)
                {
                    return NotFound($"No se encontró el torneo con ID {id}.");
                }

                return Ok(updatedTournament);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTournamentById(int id)
        {
            try
            {
                var tournament = await _tournamentService.GetTournamentByIdAsync(id);

                if (tournament == null)
                {
                    return NotFound($"No se encontró el torneo con ID {id}.");
                }

                return Ok(tournament);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateTournamentStatus(int id, [FromBody] UpdateTournamentStatusDto updateStatusDto)
        {
            try
            {
                var updatedTournament = await _tournamentService.UpdateTournamentStatusAsync(id, updateStatusDto.NewStatus);
                if (updatedTournament == null)
                {
                    return NotFound($"No se encontró el torneo con ID {id}.");
                }
                return Ok(updatedTournament);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno del servidor: {ex.Message}" });
            }
        }

        [Authorize]
        [RequirePermission(Permissions.Tournament.Join)]
        [HttpPost("{id}/enroll")]
        public async Task<IActionResult> EnrollInTournament(int id, [FromBody] TournamentEnrollmentDto enrollmentDto)
        {
            try
            {
                var enrollment = await _tournamentService.EnrollPlayerAsync(id, enrollmentDto);

                if (enrollment == null)
                {
                    return BadRequest("No se pudo procesar la inscripción.");
                }

                return Ok(enrollment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [Authorize]
        [RequirePermission(Permissions.Tournament.Join)]
        [HttpPost("{id}/enroll-with-payment")]
        public async Task<IActionResult> EnrollInTournamentWithPayment(int id, [FromBody] TournamentEnrollmentWithPaymentDto enrollmentDto)
        {
            try
            {
                var result = await _tournamentService.EnrollWithPaymentAsync(id, enrollmentDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [Authorize]
        [RequirePermission(Permissions.Tournament.Join)]
        [HttpDelete("{tournamentId}/enrollment")]
        public async Task<IActionResult> CancelEnrollment(int tournamentId)
        {
            try
            {
                var userIdClaim = User.FindFirst("user_id");
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized("No se pudo identificar al usuario.");
                }

                var result = await _tournamentService.CancelEnrollmentAsync(tournamentId, userId);

                if (!result)
                {
                    return NotFound("No se encontró la inscripción para cancelar.");
                }

                return Ok("Inscripción cancelada exitosamente.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("{id}/generate-bracket")]
        public async Task<IActionResult> GenerateTournamentBracket(
            int id, 
            [FromQuery] bool autoSchedule = false)
        {
            try
            {
                var result = await _bracketGenerationService.GenerateTournamentBracketAsync(id, autoSchedule);

                if (result == null)
                {
                    return NotFound($"No se encontró el torneo con ID {id}.");
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("{id}/bracket")]
        public async Task<IActionResult> GetTournamentBracket(int id)
        {
            try
            {
                var result = await _bracketGenerationService.GetTournamentBracketAsync(id);

                if (result == null)
                {
                    return NotFound($"No se encontró el bracket para el torneo con ID {id}.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("{id}/all-phases")]
        public async Task<IActionResult> GetAllTournamentPhases(int id)
        {
            try
            {
                var result = await _bracketGenerationService.GetAllTournamentPhasesAsync(id);

                if (result == null || result.Count == 0)
                {
                    return NotFound($"No se encontraron fases para el torneo con ID {id}.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("match/{matchId}/schedule")]
        [RequirePermission(Permissions.Tournament.AssignUser)]
        public async Task<IActionResult> AssignMatchSchedule(int matchId, [FromBody] AssignMatchScheduleDto scheduleDto)
        {
            try
            {
                scheduleDto.MatchId = matchId;

                var result = await _matchSchedulingService.AssignMatchScheduleAsync(scheduleDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpDelete("match/{matchId}/schedule")]
        [RequirePermission(Permissions.Tournament.AssignUser)]
        public async Task<IActionResult> UnassignMatchSchedule(int matchId)
        {
            try
            {
                var result = await _matchSchedulingService.UnassignMatchScheduleAsync(matchId);

                if (!result)
                {
                    return NotFound($"No se encontró el partido con ID {matchId} o no tiene horario asignado.");
                }

                return Ok("Horario del partido eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("match/{matchId}/result")]
        [Authorize]
        public async Task<IActionResult> RegisterMatchResult(int matchId, [FromBody] RegisterMatchResultDto resultDto)
        {
            try
            {
                resultDto.MatchId = matchId;

                var result = await _matchResultService.RegisterMatchResultAsync(resultDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPut("match/{matchId}/result")]
        [Authorize]
        public async Task<IActionResult> UpdateMatchResult(int matchId, [FromBody] RegisterMatchResultDto resultDto)
        {
            try
            {
                resultDto.MatchId = matchId;

                var result = await _matchResultService.UpdateMatchResultAsync(matchId, resultDto);

                if (!result)
                {
                    return NotFound($"No se encontró el partido con ID {matchId}.");
                }

                return Ok("Resultado actualizado exitosamente.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("{tournamentId}/phase/{phaseId}/auto-schedule")]
        [RequirePermission(Permissions.Tournament.AssignUser)]
        public async Task<IActionResult> AutoSchedulePhaseMatches(int tournamentId, int phaseId)
        {
            try
            {
                var phaseMatches = await _context.TournamentMatches
                    .Where(m => m.Bracket.PhaseId == phaseId)
                    .Select(m => m.Id)
                    .ToListAsync();

                if (!phaseMatches.Any())
                {
                    return NotFound($"No se encontraron partidos para la fase con ID {phaseId}.");
                }

                var result = await _autoSchedulingService.AutoScheduleMatchesAsync(phaseMatches, tournamentId);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("report")]
        [Authorize]
        public async Task<IActionResult> GetTournamentReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest("La fecha de inicio no puede ser posterior a la fecha de fin.");
                }

                var report = await _tournamentService.GetTournamentReportAsync(startDate, endDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
