using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace HealthyBites.Api.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public SmtpEmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody, string? plainTextBody = null)
        {
            if (string.IsNullOrWhiteSpace(_settings.SmtpHost))
                throw new InvalidOperationException("SMTP host is not configured. Please set EmailSettings:SmtpHost in configuration.");

            using var message = new MailMessage();
            message.From = new MailAddress(_settings.SenderEmail, _settings.SenderName);
            message.To.Add(new MailAddress(to));
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = htmlBody;

            if (!string.IsNullOrWhiteSpace(plainTextBody))
            {
                var plain = AlternateView.CreateAlternateViewFromString(plainTextBody, null, "text/plain");
                message.AlternateViews.Add(plain);
            }

            using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                EnableSsl = _settings.UseSsl
            };

            if (!string.IsNullOrWhiteSpace(_settings.Username))
            {
                client.Credentials = new NetworkCredential(_settings.Username, _settings.Password ?? string.Empty);
            }

            await client.SendMailAsync(message);
        }
    }
}
