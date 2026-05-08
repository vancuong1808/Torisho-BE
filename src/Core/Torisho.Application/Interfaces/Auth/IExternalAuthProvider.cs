using Torisho.Application.DTOs.Auth;
using Torisho.Domain.Enums;

namespace Torisho.Application.Interfaces.Auth;

public interface IExternalAuthProvider
{
    AuthProvider Provider { get; }

    Task<ExternalAuthResult> VerifyTokenAsync(string token, CancellationToken ct = default);
}