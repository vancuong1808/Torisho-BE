using Torisho.Domain.Common;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Domain.Entities.RoomDomain;

public sealed class RoomMessage : BaseEntity
{
    public string Content { get; private set; } = string.Empty;
    public DateTime SentAt { get; private set; }
    public bool IsAiMessage { get; private set; }
    public Guid RoomId { get; private set; }
    public Room? Room { get; private set; }

    public Guid? SenderId { get; private set; }
    public User? Sender { get; private set; }

    private RoomMessage() { }

    public RoomMessage(Guid roomId, string content, Guid? senderId = null)
    {
        if (roomId == Guid.Empty)
            throw new ArgumentException("RoomId cannot be empty", nameof(roomId));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required", nameof(content));

        RoomId = roomId;
        Content = content;
        SenderId = senderId;
        IsAiMessage = senderId == null;
        SentAt = DateTime.UtcNow;
    }
}