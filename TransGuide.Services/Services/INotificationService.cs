using System.Collections.Generic;
using System.Threading.Tasks;
using TransiGuide.Services.Models;

namespace TransiGuide.Services.Services
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationRequest>> GetUserNotificationsAsync(int userId);
        Task<IEnumerable<NotificationRequest>> GetUnreadNotificationsAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<NotificationRequest> SendNotificationAsync(CreateNotificationRequest dto);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<bool> DeleteNotificationAsync(int notificationId);

        Task SendWelcomeNotificationAsync(int userId);
        Task SendAppUpdateNotificationAsync(int userId, string updateMessage);
        Task SendFeedbackRequestAsync(int userId, int tripId);
    }
}