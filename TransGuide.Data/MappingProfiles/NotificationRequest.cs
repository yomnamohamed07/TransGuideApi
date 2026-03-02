
using System.ComponentModel.DataAnnotations;


namespace TransGuide.Data.MappingProfiles
{
    public class NotificationRequest
    {
        public int NotificationId { get; set; }
        public string Message { get; set; }
        public DateTime TimeSent { get; set; }
        public bool IsRead { get; set; }
        public string NotificationType { get; set; }
    }

    public class CreateNotificationRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Message { get; set; }

        [MaxLength(50)]
        public string NotificationType { get; set; } = "System";
    }
}
