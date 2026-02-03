namespace Torisho.Domain.Common;

public abstract class UserAuth : BaseEntity
{
    public string Username { get; protected set; } = default!;
    public string Email { get; protected set; } = default!;
    public string PasswordHash { get; protected set; } = default!;

    protected UserAuth() { }

    protected UserAuth(string username, string email, string passwordHash)
    {
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
