using Torisho.Domain.Common;

namespace Torisho.Domain.Entities.UserDomain;

public class RefreshToken : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public User? User { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }

    private RefreshToken() { }

    public RefreshToken(Guid userId, string token, DateTime expiresAt)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
    }

    public void Revoke(string? replacedBy = null)
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        ReplacedByToken = replacedBy;
    }

    public bool IsActive => !IsRevoked && ExpiresAt > DateTime.UtcNow;
}
