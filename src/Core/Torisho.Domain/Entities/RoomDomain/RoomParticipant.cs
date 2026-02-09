using Torisho.Domain.Common;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Domain.Entities.RoomDomain;

public sealed class RoomParticipant : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public Guid RoomId { get; private set; }
    public Room Room { get; private set; } = default!;

    public DateTime JoinedAt { get; private set; }
    public DateTime? LeftAt { get; private set; }

    private RoomParticipant() { }

    public RoomParticipant(Guid userId, Guid roomId)
    {
        UserId = userId;
        RoomId = roomId;
        JoinedAt = DateTime.UtcNow;
    }

    public void Leave()
    {
        LeftAt = DateTime.UtcNow;
    }

    public bool IsActive()
    {
        return !LeftAt.HasValue;
    }
}
