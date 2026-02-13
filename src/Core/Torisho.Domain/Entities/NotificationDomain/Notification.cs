using Torisho.Domain.Common;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Domain.Entities.NotificationDomain;

public sealed class Notification : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public bool IsRead { get; private set; }
    public Guid UserId { get; private set; }
    public User? User { get; private set; }
    public string? RelatedEntityType { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public string? ActionUrl { get; set; }
    public DateTime SentAt { get; set; }

    private Notification() { }

    public Notification(Guid userId, string title, string message, string? relatedEntityType = null, Guid? relatedEntityId = null, string? actionUrl = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required", nameof(title));
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message is required", nameof(message));

        UserId = userId;
        Title = title;
        Message = message;
        RelatedEntityType = relatedEntityType;
        RelatedEntityId = relatedEntityId;
        ActionUrl = actionUrl;
        SentAt = DateTime.UtcNow;
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}
