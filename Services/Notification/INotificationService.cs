using System.Threading.Tasks;
using System.Collections.Generic;
using padelya_api.Models.Notification;

namespace padelya_api.Services.Notification
{
    public interface INotificationService
    {
        Task<Models.Notification.Notification> SendAsync(int userId, NotificationType type, object payload);
        Task<List<Models.Notification.Notification>> ListAsync(int userId);
        Task MarkAsReadAsync(int id);
    }
}

