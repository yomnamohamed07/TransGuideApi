
using System.ComponentModel.DataAnnotations;


namespace TransGuide.Data.MappingProfiles
{
   
        public class FeedbackDto
        {
            [Required(ErrorMessage = "FullName is required")]
            public string FullName { get; set; } = null!;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Email format is invalid")]
            public string Email { get; set; } = null!;

            [Phone(ErrorMessage = "PhoneNumber format is invalid")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Reason is required")]
            public string Reason { get; set; } = null!;

            public string TimeSlot { get; set; } 

            [Required(ErrorMessage = "Message is required")]
            [MaxLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
            public string Message { get; set; } = null!;

            public string Comment { get; set; } = null!;

            [Range(1, 5, ErrorMessage = "RatingId must be between 1 and 5")]
            public int RatingId { get; set; }

            [Required]
            public int UserProfileId { get; set; }

            [Required]
            public int RouteId { get; set; }

            [Required]
            public int TripStatusId { get; set; }
        }
    }



