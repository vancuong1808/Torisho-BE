using Torisho.Domain.Common;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Domain.Entities.NotificationDomain;

public sealed class Notification : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public string Title { get; private set; } = default!;
    public string Message { get; private set; } = default!;
    public bool IsRead { get; private set; }

    public string RelatedEntityType { get; private set; } = default!;
    public Guid RelatedEntityId { get; private set; }
    public string ActionUrl { get; private set; } = default!;
    public DateTime SentAt { get; private set; }

    private Notification() { }

    public Notification(Guid userId, string title, string message, string relatedEntityType, Guid relatedEntityId, string actionUrl)
    {
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
