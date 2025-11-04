using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using padelya_api.Attributes;
using padelya_api.Services.Annual;
using System.Threading.Tasks;

namespace padelya_api.Controllers
{
    [ApiController]
    [Route("api/ranking-traces")]
    [Authorize]
    public class RankingTraceController : ControllerBase
    {
        private readonly IRankingTraceService _service;

        public RankingTraceController(IRankingTraceService service)
        {
            _service = service;
        }

        [HttpGet("player/{playerId}")]
        [RequirePermission("ranking:view_own")]
        public async Task<IActionResult> GetByPlayer(int playerId, [FromQuery] int? year = null)
        {
            var traces = await _service.GetTracesByPlayerAsync(playerId, year);
            return Ok(traces);
        }

        [HttpGet("match/{matchId}")]
        [RequirePermission("ranking:view")]
        public async Task<IActionResult> GetByMatch(int matchId, [FromQuery] string matchType = "Challenge")
        {
            var traces = await _service.GetTracesByMatchAsync(matchId, matchType);
            return Ok(traces);
        }

        [HttpGet("entry/{entryId}")]
        [RequirePermission("ranking:view")]
        public async Task<IActionResult> GetByEntry(int entryId)
        {
            var traces = await _service.GetTracesByRankingEntryAsync(entryId);
            return Ok(traces);
        }

        [HttpGet("year/{year}")]
        [RequirePermission("ranking:view")]
        public async Task<IActionResult> GetByYear(int year)
        {
            var traces = await _service.GetTracesByYearAsync(year);
            return Ok(traces);
        }

        [HttpGet("challenge/{challengeId}")]
        [RequirePermission("ranking:view")]
        public async Task<IActionResult> GetByChallenge(int challengeId)
        {
            var traces = await _service.GetTracesByChallengeAsync(challengeId);
            return Ok(traces);
        }
    }
}

