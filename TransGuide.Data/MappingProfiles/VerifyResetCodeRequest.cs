
using System.ComponentModel.DataAnnotations;


namespace TransGuide.Data.MappingProfiles
{
    public class VerifyResetCodeRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Code { get; set; } = string.Empty;
    }
}
