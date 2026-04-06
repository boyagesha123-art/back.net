using Microsoft.AspNetCore.Mvc;
using HealthyBites.Api.Models;

namespace HealthyBites.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalorieController : ControllerBase
    {
        [HttpPost("calculate")]
        public ActionResult<CalorieResult> CalculateCalories([FromBody] UserDataRequest request)
        {
            double BMR = 0;

            if(request.Gender.ToLower() == "men")
                BMR = 10 * request.WeightKg + 6.25 * request.HeightCm - 5 * request.AgeYears + 5;
            else if(request.Gender.ToLower() == "women")
                BMR = 10 * request.WeightKg + 6.25 * request.HeightCm - 5 * request.AgeYears - 161;
            else
                return BadRequest("Gender must be 'Men' or 'Women'");

            double TDEE = BMR;

            switch(request.ActivityLevel.ToLower())
            {
                case "sedentary": TDEE *= 1.2; break;
                case "lightly active": TDEE *= 1.375; break;
                case "moderately active": TDEE *= 1.55; break;
                case "very active": TDEE *= 1.725; break;
                case "extra active": TDEE *= 1.9; break;
                default: return BadRequest("Invalid activity level");
            }

            double iTDEE = TDEE;

            if(request.Goal.ToLower() == "lose")
                iTDEE -= request.GoalSpeed.ToLower() == "moderate" ? 500 : request.GoalSpeed.ToLower() == "slow" ? 300 : 0;
            else if(request.Goal.ToLower() == "gain")
                iTDEE += request.GoalSpeed.ToLower() == "fast" ? 500 : request.GoalSpeed.ToLower() == "lean" ? 300 : 0;
            // maintain => iTDEE = TDEE

            return Ok(new CalorieResult
            {
                BMR = BMR,
                TDEE = TDEE,
                iTDEE = iTDEE
            });
        }
    }
}