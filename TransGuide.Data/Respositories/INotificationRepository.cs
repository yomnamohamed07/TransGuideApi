using System.Collections.Generic;
using System.Threading.Tasks;
using TransGuide.Data.Entities.ApplicationEntities;

namespace TransiGuide.Data.Repositories
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<Notification> CreateAsync(Notification notification);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<bool> DeleteAsync(int notificationId);
        Task<Notification> GetLastNotificationAsync(int userId);
    }
}
