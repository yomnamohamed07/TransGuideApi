using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TransGuide.Data.Entities.Identity;

namespace TransGuide.Data.Entities.ApplicationEntities
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        public  int UserId { get; set; }

        [Required, MaxLength(500)]
        public required string Message { get; set; }

        [Required]
        public DateTime TimeSent { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        [MaxLength(50)]
        public string NotificationType { get; set; } = "System";

        [ForeignKey("UserId")]
        public virtual UserProfile? User { get; set; }
    }
}