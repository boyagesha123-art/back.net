namespace HealthyBites.Api.Services
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email message.
        /// </summary>
        /// <param name="to">Recipient email address.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="htmlBody">Email HTML body.</param>
        /// <param name="plainTextBody">Optional plain-text fallback body.</param>
        Task SendEmailAsync(string to, string subject, string htmlBody, string? plainTextBody = null);
    }
}
