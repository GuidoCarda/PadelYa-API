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
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Create([FromQuery] int year, [FromBody] CreateChallengeDto dto)
        {
            try
            {
                var c = await _service.CreateWithDetailsAsync(year, dto);
                return Ok(c);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("pending/{playerId}")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> GetPending(int playerId)
        {
            var list = await _service.GetPendingChallengesAsync(playerId);
            return Ok(list);
        }

        [HttpPost("{id}/respond")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Respond(int id, [FromBody] RespondChallengeDto dto)
        {
            try
            {
                var c = await _service.RespondWithDetailsAsync(id, dto.Accept);
                return Ok(c);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/result")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> RegisterResult(int id, [FromBody] RegisterChallengeResultDto dto)
        {
            try
            {
                var c = await _service.RegisterResultWithDetailsAsync(id, dto);
                return Ok(c);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("history/{playerId}")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> History(int playerId, [FromQuery] int? year = null)
        {
            var list = await _service.GetHistoryWithDetailsAsync(playerId, year);
            return Ok(list);
        }

        [HttpGet("pending-validation")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        [padelya_api.Attributes.RequirePermission("ranking:manage")]
        public async Task<IActionResult> GetPendingValidation()
        {
            var list = await _service.GetChallengesRequiringValidationAsync();
            return Ok(list);
        }

        [HttpPut("{id}/validate")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        [padelya_api.Attributes.RequirePermission("ranking:manage")]
        public async Task<IActionResult> Validate(int id, [FromBody] RegisterChallengeResultDto dto)
        {
            try
            {
                var adminUserId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
                var c = await _service.ValidateWithDetailsAsync(id, dto, adminUserId);
                return Ok(c);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

