namespace HealthyBites.Api.Dtos
{
    public class RegisterDto
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }

        public int Age { get; set; }
        public string Phone { get; set; } = string.Empty;
        public double Height { get; set; }
        public double Weight { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string ActivityLevel { get; set; } = string.Empty;
        public string SmokingStatus { get; set; } = string.Empty;
        public int SleepHours { get; set; }
        public string StressLevel { get; set; } = string.Empty;
        public bool HasType2Diabetes { get; set; }
    }
}
