namespace Torisho.Application.DTOs.Auth;

public class ExternalAuthResult
{
    public string ProviderId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool IsEmailVerified { get; set; }
}