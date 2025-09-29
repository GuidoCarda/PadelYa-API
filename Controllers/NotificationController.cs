using Microsoft.AspNetCore.Mvc;
using padelya_api.Services.Notification;

namespace padelya_api.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;
        public NotificationController(INotificationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int userId)
        {
            var list = await _service.ListAsync(userId);
            return Ok(list);
        }

        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _service.MarkAsReadAsync(id);
            return NoContent();
        }
    }
}

