namespace Torisho.Domain.Common;

public abstract class UserAuth : BaseEntity
{
    public string Username { get; protected set; } = string.Empty;
    public string Email { get; protected set; } = string.Empty;
    public string PasswordHash { get; protected set; } = string.Empty;

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
    }

    public virtual bool Authenticate(string password)
        => string.Equals(PasswordHash, password, StringComparison.Ordinal);

    public virtual bool ValidateEmail()
        => Email.Contains("@", StringComparison.Ordinal);

    public abstract bool HasPermission(string code);
}
