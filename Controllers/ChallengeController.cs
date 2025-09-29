using Microsoft.AspNetCore.Mvc;
using padelya_api.DTOs.Challenge;
using padelya_api.Services.Annual;

namespace padelya_api.Controllers
{
    [ApiController]
    [Route("api/challenges")]
    public class ChallengeController : ControllerBase
    {
        private readonly IChallengeService _service;
        public ChallengeController(IChallengeService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] int year, [FromBody] CreateChallengeDto dto)
        {
            var c = await _service.CreateAsync(year, dto);
            return Ok(c);
        }

        [HttpPost("{id}/respond")]
        public async Task<IActionResult> Respond(int id, [FromBody] RespondChallengeDto dto)
        {
            var c = await _service.RespondAsync(id, dto.Accept);
            return Ok(c);
        }

        [HttpPost("{id}/result")]
        public async Task<IActionResult> RegisterResult(int id, [FromBody] RegisterChallengeResultDto dto)
        {
            var c = await _service.RegisterResultAsync(id, dto);
            return Ok(c);
        }

        [HttpGet("history/{playerId}")]
        public async Task<IActionResult> History(int playerId, [FromQuery] int? year = null)
        {
            var list = await _service.GetHistoryAsync(playerId, year);
            return Ok(list);
        }

        [HttpPut("{id}/validate")]
        public async Task<IActionResult> Validate(int id, [FromBody] RegisterChallengeResultDto dto)
        {
            var c = await _service.ValidateAsync(id, dto);
            return Ok(c);
        }
    }
}

