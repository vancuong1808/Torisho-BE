using Torisho.Domain.Common;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Entities.RoomDomain;

public sealed class Room : BaseEntity, IAggregateRoot
{
    private readonly HashSet<RoomParticipant> _participants = new();

    public RoomStatus Status { get; private set; }
    public RoomType RoomType { get; private set; }
    public Guid? AiCoachId { get; private set; }
    public string? Transcript { get; private set; }

    public DateTime ScheduledAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }

    public IReadOnlyCollection<RoomParticipant> Participants => _participants;

    private Room() { }

    public Room(RoomType roomType, DateTime scheduledAt, Guid? aiCoachId = null)
    {
        RoomType = roomType;
        ScheduledAt = scheduledAt;
        AiCoachId = aiCoachId;
        Status = RoomStatus.Waiting;
    }
}
