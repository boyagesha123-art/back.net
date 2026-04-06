namespace HealthyBites.Api.Dtos
{
    public class WeightDynamicsRequestDto
    {
        public Guid UserId { get; set; }
        public double DailyEnergyIntake { get; set; } // kcal/day
    }
}