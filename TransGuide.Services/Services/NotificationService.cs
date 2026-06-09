
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.MappingProfiles;
using TransGuide.Data.Services;
using TransiGuide.Data.Repositories; 


namespace TransiGuide.Services.Services
{
    public class NotificationService : INotificationService 
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<IEnumerable<NotificationRequest>> GetUserNotificationsAsync(int userId)
        {
            var notifications = await _notificationRepository.GetUserNotificationsAsync(userId);
            return notifications.Select(MapToDto);
        }

        public async Task<IEnumerable<NotificationRequest>> GetUnreadNotificationsAsync(int userId)
        {
            var notifications = await _notificationRepository.GetUnreadNotificationsAsync(userId);
            return notifications.Select(MapToDto);
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _notificationRepository.GetUnreadCountAsync(userId);
        }

        public async Task<NotificationRequest> SendNotificationAsync(CreateNotificationRequest dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Message))
                throw new ArgumentException("Notification message cannot be empty");

            var notification = new Notification
            {
                UserId = dto.UserId,
                Message = dto.Message,
                NotificationType = dto.NotificationType ?? "System",
                TimeSent = DateTime.UtcNow,
                IsRead = false
            };

            var created = await _notificationRepository.CreateAsync(notification);
            return MapToDto(created);
        }

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            return await _notificationRepository.MarkAsReadAsync(notificationId);
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            return await _notificationRepository.MarkAllAsReadAsync(userId);
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId)
        {
            return await _notificationRepository.DeleteAsync(notificationId);
        }

        public async Task SendWelcomeNotificationAsync(int userId)
        {
            var welcomeDto = new CreateNotificationRequest
            {
                UserId = userId,
                Message = "Welcome to TransiGuide! 🚌 Start planning your journey and find the nearest station to your destination.",
                NotificationType = "Welcome"
            };

            await SendNotificationAsync(welcomeDto);
        }

        public async Task SendAppUpdateNotificationAsync(int userId, string updateMessage)
        {
            var updateDto = new CreateNotificationRequest
            {
                UserId = userId,
                Message = updateMessage ?? "🎉 TransiGuide has been updated! Check out new features and improvements.",
                NotificationType = "AppUpdate"
            };

            await SendNotificationAsync(updateDto);
        }

        public async Task SendFeedbackRequestAsync(int userId, int tripId)
        {
            var feedbackDto = new CreateNotificationRequest
            {
                UserId = userId,
                Message = $"⭐ How was your trip? Share your feedback to help us improve TransiGuide!",
                NotificationType = "FeedbackRequest"
            };

            await SendNotificationAsync(feedbackDto);
        }

        private NotificationRequest MapToDto(Notification notification)
        {
            return new NotificationRequest
            {
                NotificationId = notification.Id,
                Message = notification.Message,
                TimeSent = notification.TimeSent,
                IsRead = notification.IsRead,
                NotificationType = notification.NotificationType
            };
        }
    }
}