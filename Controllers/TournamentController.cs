using Microsoft.AspNetCore.Mvc;
using padelya_api.DTOs.Tournament;
using padelya_api.Services;
using System;
using System.Threading.Tasks;

namespace padelya_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
    }
}
