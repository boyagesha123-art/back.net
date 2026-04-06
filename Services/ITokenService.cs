using HealthyBites.Api.Models;
using System;

namespace HealthyBites.Api.Services
{
    public interface ITokenService
    {
        string CreateAccessToken(AppUser user);
        (string token, DateTime expiry) CreateRefreshToken();
    }
}