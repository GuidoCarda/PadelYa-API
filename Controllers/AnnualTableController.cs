using Microsoft.AspNetCore.Mvc;
using padelya_api.Models.Annual;
using padelya_api.Services.Annual;
using padelya_api.DTOs.Annual;
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
        [Microsoft.AspNetCore.Authorization.Authorize]
        [padelya_api.Attributes.RequirePermission("ranking:view")]
        public async Task<IActionResult> Export([FromQuery] int year)
        {
            var ranking = await _service.GetRankingWithNamesAsync(year);
            var sb = new StringBuilder();
            sb.AppendLine("Position,PlayerId,PlayerName,PlayerSurname,PointsTotal,Wins,Losses,Draws,FromTournaments,FromChallenges,FromClasses,FromMatchWins,FromMatchLosses");
            foreach (var r in ranking)
            {
                sb.AppendLine(string.Join(',', new object[]
                {
                    r.Position, r.PlayerId, r.PlayerName ?? "", r.PlayerSurname ?? "", 
                    r.PointsTotal, r.Wins, r.Losses, r.Draws,
                    r.PointsFromTournaments, r.PointsFromChallenges, r.PointsFromClasses,
                    r.PointsFromMatchWins, r.PointsFromMatchLosses
                }));
            }
            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"ranking_{year}.csv");
        }

        [HttpGet]
        public async Task<IActionResult> GetRanking([FromQuery] int year, [FromQuery] int? playerId = null, [FromQuery] string? from = null, [FromQuery] string? to = null, [FromQuery] bool includeNames = false)
        {
            if (includeNames)
            {
                var ranking = await _service.GetRankingWithNamesAsync(year, playerId, from, to);
                return Ok(ranking);
            }
            var rankingBasic = await _service.GetRankingAsync(year, playerId, from, to);
            return Ok(rankingBasic);
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus([FromQuery] int year)
        {
            var status = await _service.GetStatusAsync(year);
            return Ok(new { status });
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics([FromQuery] int year)
        {
            var statistics = await _service.GetStatisticsAsync(year);
            return Ok(statistics);
        }

        [HttpPatch("status")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        [padelya_api.Attributes.RequirePermission("ranking:manage")]
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
        [Microsoft.AspNetCore.Authorization.Authorize]
        [padelya_api.Attributes.RequirePermission("ranking:manage")]
        public async Task<IActionResult> UpsertRules([FromQuery] int year, [FromBody] List<padelya_api.DTOs.Annual.ScoringRuleDto> rules)
        {
            var saved = await _service.UpsertScoringRulesAsync(year, rules);
            return Ok(saved);
        }

        /// <summary>
        /// Obtener reporte de tabla anual y desafíos con estadísticas y gráficos
        /// </summary>
        [HttpGet("report")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        [padelya_api.Attributes.RequirePermission("ranking:view")]
        public async Task<IActionResult> GetAnnualTableReport([FromQuery] string startDate, [FromQuery] string endDate)
        {
            try
            {
                if (!DateTime.TryParseExact(startDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedStartDate))
                {
                    return BadRequest(new { message = "El formato de fecha inicial debe ser YYYY-MM-DD" });
                }

                if (!DateTime.TryParseExact(endDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedEndDate))
                {
                    return BadRequest(new { message = "El formato de fecha final debe ser YYYY-MM-DD" });
                }

                if (parsedStartDate > parsedEndDate)
                {
                    return BadRequest(new { message = "La fecha inicial no puede ser mayor a la fecha final" });
                }

                var report = await _service.GetAnnualTableReportAsync(parsedStartDate, parsedEndDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error al generar reporte: {ex.Message}" });
            }
        }
    }
}

