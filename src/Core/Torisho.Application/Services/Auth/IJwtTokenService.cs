using System.Security.Claims;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Application.Services.Auth;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
    DateTime GetTokenExpiry();
}