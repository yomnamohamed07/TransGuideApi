using System.ComponentModel.DataAnnotations;

namespace TransGuideApi.Models
{
    public class RegisterRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? Country { get; set; }
        public string? Address { get; set; }

        public decimal CurrentLatitude { get; set; } = 30.0m;
        public decimal CurrentLongitude { get; set; } = 31.0m;
    }
}