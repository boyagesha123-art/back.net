using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthyBites.Api.Data;
using HealthyBites.Api.Dtos;
using HealthyBites.Api.Models;
using HealthyBites.Api.Services;
using System.Security.Cryptography;
using System.Text;

namespace HealthyBites.Api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AuthController(AppDbContext context, ITokenService tokenService, IEmailService emailService)
        {
            _context = context;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        // REGISTER
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.AppUsers.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new { message = "Email already exists" });

            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                Age = dto.Age,
                Height = dto.Height,
                Weight = dto.Weight,
                Gender = dto.Gender,
                ActivityLevel = dto.ActivityLevel,
                SmokingStatus = dto.SmokingStatus,
                SleepHours = dto.SleepHours,
                StressLevel = dto.StressLevel,
                HasType2Diabetes = dto.HasType2Diabetes
            };

            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();

            var accessToken = _tokenService.CreateAccessToken(user);
            var (refreshToken, refreshExpiry) = _tokenService.CreateRefreshToken();

            return Ok(new LoginResponseDto
            {
                Message = "User registered successfully",
                UserId = user.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = refreshExpiry
            });
        }

        // LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var hashedPassword = HashPassword(dto.Password);

            var user = await _context.AppUsers
                .FirstOrDefaultAsync(u =>
                    u.Email == dto.Email &&
                    u.PasswordHash == hashedPassword);

            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            var accessToken = _tokenService.CreateAccessToken(user);
            var (refreshToken, refreshExpiry) = _tokenService.CreateRefreshToken();

            return Ok(new LoginResponseDto
            {
                Message = "Login successful",
                UserId = user.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = refreshExpiry
            });
        }

        // FORGET PASSWORD (OTP via email)
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordDto dto)
        {
            // Intentionally return a generic response to avoid email enumeration.
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user != null)
            {
                var otp = GenerateOtp(6);

                user.ResetPasswordToken = otp;
                user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);

                await _context.SaveChangesAsync();

                var subject = "Your HealthyBites password reset code";
                var htmlBody = $@"<p>Hi {user.FullName},</p>
<p>You requested a password reset. Use the code below to reset your password. The code expires in 15 minutes.</p>
<h2 style=""letter-spacing: 2px;"">{otp}</h2>
<p>If you did not request this, please ignore this email.</p>";

                var plainTextBody = $"Hi {user.FullName},\n\nUse the code below to reset your password (expires in 15 minutes):\n\n{otp}\n\nIf you did not request this, please ignore this email.";

                await _emailService.SendEmailAsync(user.Email, subject, htmlBody, plainTextBody);
            }

            return Ok(new { message = "If an account with that email exists, a reset code has been sent." });
        }

        [HttpPost("verify-reset-code")]
        public async Task<IActionResult> VerifyResetCode(VerifyResetCodeDto dto)
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(u =>
                u.Email == dto.Email &&
                u.ResetPasswordToken == dto.Code &&
                u.ResetTokenExpiry > DateTime.UtcNow);

            if (user == null)
                return BadRequest(new { message = "Invalid or expired code" });

            return Ok(new { message = "Code is valid" });
        }


       
        // ================= RESET PASSWORD =================
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var user = await _context.AppUsers.FirstOrDefaultAsync(u =>
            u.ResetPasswordToken == dto.Token &&
            u.ResetTokenExpiry > DateTime.UtcNow);

        if (user == null)
            return BadRequest(new { message = "Invalid or expired token" });

        user.PasswordHash = HashPassword(dto.NewPassword);
        user.ResetPasswordToken = null;
        user.ResetTokenExpiry = null;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Password reset successfully" });
    }


        // Generate a time-based OTP code (numeric) for password reset flows.
        private static string GenerateOtp(int digits = 6)
        {
            if (digits < 1)
                throw new ArgumentOutOfRangeException(nameof(digits), "OTP digit count must be at least 1.");

            var buffer = new byte[digits];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(buffer);

            var sb = new StringBuilder(digits);
            for (var i = 0; i < digits; i++)
            {
                sb.Append((buffer[i] % 10).ToString());
            }

            return sb.ToString();
        }

        // Simple password hashing
        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}