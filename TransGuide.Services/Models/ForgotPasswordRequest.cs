using System.ComponentModel.DataAnnotations;

namespace TransGuide.Services.Models
{
    public class ForgotPasswordRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}