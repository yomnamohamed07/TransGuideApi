

using System.ComponentModel.DataAnnotations;

namespace TransGuide.Data.Entities.ApplicationEntities
{
    public class SignSession
    {
        [Key]
        public Guid SessionId { get; set; }

        public string? Word { get; set; }
        public DateTime LastFrameTime { get; set; } = DateTime.Now;
        public bool IsEnded { get; set; } = false;
    }
}
