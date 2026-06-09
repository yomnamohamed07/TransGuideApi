
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using System.Net.Mail;
using TransGuide.Data.MappingProfiles;
using TransGuide.Data.Services;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace TransGuide.Services.Services
{
    
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(EmailSettings settings)
        {
            _settings = settings;
        }

        public async Task<string> SendEmail(string to, string message, string? subject)
        {
            try
            {
                using var client = new SmtpClient();

                await client.ConnectAsync(
                    _settings.Host,
                    _settings.Port,
                    SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(
                    _settings.FromEmail,
                    _settings.Password
                );

                var email = new MimeMessage();

                email.From.Add(new MailboxAddress("TransGuide App", _settings.FromEmail));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject ?? "Notification";

                email.Body = new BodyBuilder
                {
                    HtmlBody = message
                }.ToMessageBody();

                await client.SendAsync(email);
                await client.DisconnectAsync(true);

                return "Success";
            }
            catch
            {
                return "Failed";
            }
        }
    }
}
