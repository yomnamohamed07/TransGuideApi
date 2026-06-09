

namespace TransGuide.Data.Services
{
    public interface IEmailService
    {
        Task<string> SendEmail(string to, string message, string? subject);
    }
}
