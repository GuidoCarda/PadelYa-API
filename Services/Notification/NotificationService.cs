using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using padelya_api.Data;
using padelya_api.Models.Notification;

namespace padelya_api.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly PadelYaDbContext _context;
        public NotificationService(PadelYaDbContext context)
        {
            _context = context;
        }

        public async Task<Models.Notification.Notification> SendAsync(int userId, NotificationType type, object payload)
        {
            var n = new Models.Notification.Notification
            {
                UserId = userId,
                Type = type,
                Payload = JsonSerializer.Serialize(payload)
            };
            _context.Add(n);
            await _context.SaveChangesAsync();
            return n;
        }

        public async Task<List<Models.Notification.Notification>> ListAsync(int userId)
        {
            return await _context.Set<Models.Notification.Notification>()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(int id)
        {
            var n = await _context.Set<Models.Notification.Notification>().FindAsync(id);
            if (n == null) return;
            n.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}

