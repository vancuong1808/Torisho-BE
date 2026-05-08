namespace Torisho.Application.DTOs.Auth;

public class ExternalLoginRequest
{
    public string Provider { get; set; } = string.Empty;
    public string ProviderToken { get; set; } = string.Empty;
}