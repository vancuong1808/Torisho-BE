using Torisho.Domain.Enums;

namespace Torisho.Domain.Common;

public abstract class UserAuth : BaseEntity
{
    public string Username { get; protected set; } = string.Empty;
    public string Email { get; protected set; } = string.Empty;
    public string? PasswordHash { get; protected set; }
    public AuthProvider AuthProvider { get; protected set; } = AuthProvider.Local;
    public string? AuthProviderId { get; protected set; }

    protected UserAuth() { }

    protected UserAuth(string username, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required", nameof(username));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("PasswordHash is required", nameof(passwordHash));

        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        AuthProvider = AuthProvider.Local;
        AuthProviderId = null;
    }

    protected UserAuth(string username, string email, AuthProvider provider, string providerId)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required", nameof(username));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));
        if (provider == AuthProvider.Local)
            throw new ArgumentException("Provider must be external", nameof(provider));
        if (string.IsNullOrWhiteSpace(providerId))
            throw new ArgumentException("ProviderId is required", nameof(providerId));

        Username = username;
        Email = email;
        PasswordHash = null;
        AuthProvider = provider;
        AuthProviderId = providerId;
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("New password hash is required", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
    }

    public bool IsExternalUser => AuthProvider != AuthProvider.Local;

    public virtual bool Authenticate(string password)
    {
        throw new NotImplementedException("Should use password hashing service");
    }

    public virtual bool ValidateEmail()
        => Email.Contains("@", StringComparison.Ordinal);

    public abstract bool HasPermission(string code);
}
