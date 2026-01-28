using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransGuide.Data.Entities.ApplicationEntities
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty; // Identity User ID
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
