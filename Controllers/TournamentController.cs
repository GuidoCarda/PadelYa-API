using Microsoft.AspNetCore.Mvc;
using padelya_api.DTOs.Tournament;
using padelya_api.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace padelya_api.Controllers
{
    [ApiController]
    [Route("api/Tournament")]
    public class TournamentController(ITournamentService tournamentService) : ControllerBase
    {
        private readonly ITournamentService _tournamentService = tournamentService;

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
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("{id}/enroll")]
        public async Task<IActionResult> EnrollInTournament(int id, [FromBody] TournamentEnrollmentDto enrollmentDto)
        {
            try
            {
                // El servicio se encargará de obtener el ID del usuario actual desde el token.
                var enrollment = await _tournamentService.EnrollPlayerAsync(id, enrollmentDto);

                if (enrollment == null)
                {
                    return BadRequest("No se pudo procesar la inscripción.");
                }

                return Ok(enrollment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Para errores de validación (ej: "cupos llenos")
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
