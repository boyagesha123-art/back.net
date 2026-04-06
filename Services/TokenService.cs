using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using HealthyBites.Api.Models;
using HealthyBites.Api.Services;
using Microsoft.Extensions.Configuration;

namespace HealthyBites.Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly string _key;
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
            _key = _config["Jwt:Key"] ?? string.Empty;

            // Validate key length for HS256 (requires >= 256 bits => 32 bytes)
            if (string.IsNullOrWhiteSpace(_key) || Encoding.UTF8.GetByteCount(_key) < 32)
            {
                throw new ArgumentOutOfRangeException(nameof(_key), "Jwt:Key in configuration must be at least 32 bytes (>= 256 bits). Please set a sufficiently long secret in appsettings.json (Jwt:Key). For local tests you can use a 32+ character random string.");
            }
        }

        public string GenerateToken(Claim[] claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "HealthyBites",
                audience: "HealthyBites",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        public string CreateAccessToken(AppUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("name", user.FullName ?? string.Empty)
            };

            return GenerateToken(claims);
        }

        public (string token, DateTime expiry) CreateRefreshToken()
        {
            var token = GenerateRefreshToken();
            var expiry = DateTime.UtcNow.AddDays(7);
            return (token, expiry);
        }
    }
}

