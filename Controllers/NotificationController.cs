using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using padelya_api.Services.Notification;

namespace padelya_api.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;
        public NotificationController(INotificationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var userId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
            var list = await _service.ListAsync(userId);
            return Ok(list);
        }

        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _service.MarkAsReadAsync(id);
            return NoContent();
        }

        [HttpPatch("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
            var notifications = await _service.ListAsync(userId);
            foreach (var notification in notifications.Where(n => n.ReadAt == null))
            {
                await _service.MarkAsReadAsync(notification.Id);
            }
            return NoContent();
        }
    }
}

