using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Torisho.Application.DTOs.Auth;
using Torisho.Application.Interfaces.Auth;
using Torisho.Domain.Enums;

namespace Torisho.Infrastructure.Services.Auth;

public class GoogleAuthProvider : IExternalAuthProvider
{
    private readonly string _clientId;
    private readonly IHostEnvironment _hostEnvironment;

    public GoogleAuthProvider(IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;

        _clientId = (configuration["Authentication:Google:ClientId"]
            ?? throw new InvalidOperationException("Google ClientId not configured")).Trim();

        if (string.IsNullOrWhiteSpace(_clientId))
            throw new InvalidOperationException("Google ClientId is empty");
    }

    public AuthProvider Provider => AuthProvider.Google;

    public async Task<ExternalAuthResult> VerifyTokenAsync(string token, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new UnauthorizedAccessException("Google token is required");

        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_clientId],
                // Some local development machines are a few minutes behind/ahead.
                // Tolerances avoid false negatives while still enforcing token expiry.
                IssuedAtClockTolerance = TimeSpan.FromMinutes(10),
                ExpirationTimeClockTolerance = TimeSpan.FromMinutes(2)
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);

            if (string.IsNullOrWhiteSpace(payload.Email))
                throw new UnauthorizedAccessException("Google account does not provide an email");

            return new ExternalAuthResult
            {
                ProviderId = payload.Subject,
                Email = payload.Email,
                FullName = string.IsNullOrWhiteSpace(payload.Name)
                    ? payload.Email.Split('@')[0]
                    : payload.Name,
                AvatarUrl = payload.Picture,
                IsEmailVerified = payload.EmailVerified
            };
        }
        catch (InvalidJwtException ex)
        {
            if (_hostEnvironment.IsDevelopment())
                throw new UnauthorizedAccessException($"Invalid Google token: {ex.Message}", ex);

            throw new UnauthorizedAccessException("Invalid Google token", ex);
        }
    }
}