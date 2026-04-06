using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthyBites.Api.Data;
using HealthyBites.Api.Dtos;
using HealthyBites.Api.Services;

namespace HealthyBites.Api.Controllers
{
    [ApiController]
    [Route("api/v1/weight-dynamics")]
    public class WeightDynamicsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWeightDynamicsService _weightDynamicsService;

        public WeightDynamicsController(AppDbContext context, IWeightDynamicsService weightDynamicsService)
        {
            _context = context;
            _weightDynamicsService = weightDynamicsService;
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateWeightChange(WeightDynamicsRequestDto request)
        {
            var user = await _context.AppUsers.FindAsync(request.UserId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var result = _weightDynamicsService.CalculateWeightChange(user, request.DailyEnergyIntake);

            return Ok(result);
        }
    }
}