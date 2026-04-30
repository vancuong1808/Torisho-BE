using Torisho.Domain.Common;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Entities.UserDomain;

public class UserToken : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public User? User { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public UserTokenType Type { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTime? UsedAt { get; private set; }

    private UserToken() { }

    public UserToken(Guid userId, string tokenHash, UserTokenType type, DateTime expiresAt)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required", nameof(userId));
        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new ArgumentException("TokenHash is required", nameof(tokenHash));
        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("ExpiresAt must be in the future", nameof(expiresAt));

        UserId = userId;
        TokenHash = tokenHash;
        Type = type;
        ExpiresAt = expiresAt;
        IsUsed = false;
    }

    public bool IsActive => !IsUsed && ExpiresAt > DateTime.UtcNow;

    public void MarkUsed()
    {
        if (IsUsed)
            return;

        IsUsed = true;
        UsedAt = DateTime.UtcNow;
    }
}
