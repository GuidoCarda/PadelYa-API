using Microsoft.AspNetCore.Mvc;
using padelya_api.Services;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace padelya_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourtSlotsController : ControllerBase
    {
        private readonly ICourtSlotService _courtSlotService;

        public CourtSlotsController(ICourtSlotService courtSlotService)
        {
            _courtSlotService = courtSlotService;
        }

        // GET: api/courtslots/occupied?courtId=1&startDate=2024-07-01&endDate=2024-07-07
        [HttpGet("occupied")]
        public async Task<IActionResult> GetOccupiedSlots([FromQuery] int courtId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var slots = await _courtSlotService.GetSlotsByDateRangeAsync(courtId, startDate, endDate);
            return Ok(slots);
        }

        // GET: api/courtslots/available?courtId=1&startDate=2024-07-01&endDate=2024-07-07
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] int courtId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var slots = await _courtSlotService.GetAvailableSlotsByDateRangeAsync(courtId, startDate, endDate);
            return Ok(slots.Select(s => new
            {
                Date = s.Date.ToString("yyyy-MM-dd"),
                Start = s.Start.ToString("HH:mm"),
                End = s.End.ToString("HH:mm")
            }));
        }
    }
}
