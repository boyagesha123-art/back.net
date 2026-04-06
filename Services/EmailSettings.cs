namespace HealthyBites.Api.Services
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public bool UseSsl { get; set; } = true;
        public string SenderName { get; set; } = "HealthyBites";
        public string SenderEmail { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
