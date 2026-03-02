

namespace TransGuide.Services.DTOS
{
        public class UserFeedback
        {
            public int Id { get; set; }

          
            public string FullName { get; set; } = string.Empty;


             public string Email { get; set; } = string.Empty;
            
            public string PhoneNumber { get; set; } = string.Empty;

           
            public string Reason { get; set; } = string.Empty;

           
            public string TimeSlot { get; set; } = string.Empty;

          
            public string Message { get; set; } = string.Empty;

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }

    }

