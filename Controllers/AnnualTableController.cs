using Microsoft.AspNetCore.Mvc;
using padelya_api.Models.Annual;
using padelya_api.Services.Annual;
using System.Text;

namespace padelya_api.Controllers
{
    [ApiController]
    [Route("api/annual-table")]
    public class AnnualTableController : ControllerBase
    {
        private readonly IAnnualTableService _service;

        public AnnualTableController(IAnnualTableService service)
        {
            _service = service;
        }

        [HttpGet("export")]
        public async Task<IActionResult> Export([FromQuery] int year)
        {
            var ranking = await _service.GetRankingAsync(year);
            var sb = new StringBuilder();
            sb.AppendLine("Position,PlayerId,PointsTotal,Wins,Losses,Draws,FromTournaments,FromChallenges,FromClasses");
            int pos = 1;
            foreach (var r in ranking)
            {
                sb.AppendLine(string.Join(',', new object[]
                {
                    pos++, r.PlayerId, r.PointsTotal, r.Wins, r.Losses, r.Draws,
                    r.PointsFromTournaments, r.PointsFromChallenges, r.PointsFromClasses
                }));
            }
            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"ranking_{year}.csv");
        }

        [HttpGet]
        public async Task<IActionResult> GetRanking([FromQuery] int year, [FromQuery] int? playerId = null, [FromQuery] string? from = null, [FromQuery] string? to = null)
        {
            var ranking = await _service.GetRankingAsync(year, playerId, from, to);
            return Ok(ranking);
        }

        [HttpPatch("status")]
        public async Task<IActionResult> UpdateStatus([FromQuery] int year, [FromBody] AnnualTableStatus status)
        {
            var table = await _service.UpdateStatusAsync(year, status);
            return Ok(table);
        }

        [HttpGet("rules")]
        public async Task<IActionResult> GetRules([FromQuery] int year)
        {
            var rules = await _service.GetScoringRulesAsync(year);
            return Ok(rules);
        }

        [HttpPut("rules")]
        public async Task<IActionResult> UpsertRules([FromQuery] int year, [FromBody] List<ScoringRule> rules)
        {
            var saved = await _service.UpsertScoringRulesAsync(year, rules);
            return Ok(saved);
        }
    }
}

