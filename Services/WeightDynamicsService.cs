using HealthyBites.Api.Dtos;
using HealthyBites.Api.Models;

namespace HealthyBites.Api.Services
{
    public class WeightDynamicsService : IWeightDynamicsService
    {
        public WeightDynamicsResponseDto CalculateWeightChange(AppUser user, double dailyEnergyIntake)
        {
            // Step 1: Calculate BMR
            double bmr = CalculateBMR(user.Weight, user.Height, user.Age, user.Gender);

            // Step 2: Calculate TDEE
            double activityFactor = GetActivityFactor(user.ActivityLevel);
            double tdee = bmr * activityFactor;

            // Step 3: Daily Energy Balance
            double ebDaily = tdee - dailyEnergyIntake;

            // Step 4: 30-Day Cumulative Energy Balance
            double eb30 = ebDaily * 30;

            // Step 5: Theoretical Weight Change
            double deltaWTheoretical = eb30 / 7700;

            // Step 6: Metabolic Adaptation Coefficient (α)
            double alpha = CalculateAlpha(user);

            double deltaWAlpha = deltaWTheoretical * alpha;

            // Step 7: Lifestyle Adjustment Coefficient (β)
            double beta = CalculateBeta(user);

            double deltaWFinal = deltaWAlpha * beta;

            // Step 8: Output Range
            double deltaWMin = deltaWFinal * 0.9;
            double deltaWMax = deltaWFinal * 1.1;

            return new WeightDynamicsResponseDto
            {
                ExpectedWeightChange = Math.Round(deltaWFinal, 2),
                MinWeightChange = Math.Round(deltaWMin, 2),
                MaxWeightChange = Math.Round(deltaWMax, 2)
            };
        }

        private double CalculateBMR(double weight, double height, double age, string gender)
        {
            if (gender.ToLower() == "male")
            {
                return (10 * weight) + (6.25 * height) - (5 * age) + 5;
            }
            else if (gender.ToLower() == "female")
            {
                return (10 * weight) + (6.25 * height) - (5 * age) - 161;
            }
            else
            {
                // Default to male if not specified
                return (10 * weight) + (6.25 * height) - (5 * age) + 5;
            }
        }

        private double GetActivityFactor(string activityLevel)
        {
            return activityLevel.ToLower() switch
            {
                "sedentary" => 1.20,
                "lightly active" => 1.375,
                "moderately active" => 1.55,
                "very active" => 1.725,
                _ => 1.20 // default
            };
        }

        private double CalculateAlpha(AppUser user)
        {
            // Calculate BMI
            double bmi = user.Weight / Math.Pow(user.Height / 100, 2);

            double alpha = bmi switch
            {
                < 25 => 1.00,
                < 30 => 0.95,
                < 35 => 0.90,
                _ => 0.85
            };

            // Adjustments
            if (user.ActivityLevel.ToLower() == "sedentary")
            {
                alpha -= 0.05;
            }
            else if (user.ActivityLevel.ToLower() == "very active")
            {
                alpha += 0.05;
            }

            if (user.HasType2Diabetes)
            {
                alpha -= 0.05;
            }

            // Clamp alpha between 0.75 and 1.10
            alpha = Math.Max(0.75, Math.Min(1.10, alpha));

            return alpha;
        }

        private double CalculateBeta(AppUser user)
        {
            double betaSmoking = user.SmokingStatus.ToLower() switch
            {
                "non-smoker" => 1.00,
                "light smoker" => 1.03,
                "heavy smoker" => 1.05,
                "recently quit" => 0.95,
                _ => 1.00
            };

            double betaSleep = user.SleepHours switch
            {
                >= 7 and <= 8 => 1.00,
                >= 5 and <= 6 => 0.95,
                < 5 => 0.90,
                _ => 0.90
            };

            double betaStress = user.StressLevel.ToLower() switch
            {
                "low" => 1.00,
                "moderate" => 0.97,
                "high" => 0.92,
                _ => 1.00
            };

            return betaSmoking * betaSleep * betaStress;
        }
    }
}