using TransGuide.Data.Entities.Identity;

namespace TransGuide.Data.Entities.ApplicationEntities
{
    public class Notification
    {
        public int Id { get; set; }

        public string Message { get; set; } = null!;

        public DateTime TimeSent { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        public string NotificationType { get; set; } = "System";

        public int UserId { get; set; }
        public  UserProfile User { get; set; } = null!;
    }
}