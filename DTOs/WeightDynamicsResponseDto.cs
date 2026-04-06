namespace HealthyBites.Api.Dtos
{
    public class WeightDynamicsResponseDto
    {
        public double ExpectedWeightChange { get; set; } // kg
        public double MinWeightChange { get; set; } // kg
        public double MaxWeightChange { get; set; } // kg
    }
}