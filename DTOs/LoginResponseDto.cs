namespace HealthyBites.Api.Dtos
{
    public class LoginResponseDto
    {
        public string Message { get; set; } = "Login successful";
        public Guid UserId { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiry { get; set; }
    }
}