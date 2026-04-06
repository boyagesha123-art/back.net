using HealthyBites.Api.Dtos;
using HealthyBites.Api.Models;

namespace HealthyBites.Api.Services
{
    public interface IWeightDynamicsService
    {
        WeightDynamicsResponseDto CalculateWeightChange(AppUser user, double dailyEnergyIntake);
    }
}