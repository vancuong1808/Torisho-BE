using Torisho.Domain.Common;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Entities.RoomDomain;

public sealed class Room : BaseEntity, IAggregateRoot
{
    public RoomStatus Status { get; private set; }
    public RoomType RoomType { get; private set; }
    public Guid? AiCoachId { get; private set; }
    public string? Transcript { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }

    // DDD: Aggregate - Room manages Participants through domain methods
    private readonly HashSet<RoomParticipant> _participants = new();
    public IReadOnlyCollection<RoomParticipant> Participants => _participants;
    private readonly HashSet<RoomMessage> _messages = new();
    public IReadOnlyCollection<RoomMessage> Messages => _messages;

    private Room() { }

    public Room(RoomType roomType, DateTime scheduledAt, Guid? aiCoachId = null)
    {
        RoomType = roomType;
        ScheduledAt = scheduledAt;
        AiCoachId = aiCoachId;
        Status = RoomStatus.Waiting;
    }

    public void Open()
    {
        Status = RoomStatus.Waiting;
    }

    public void Close()
    {
        Status = RoomStatus.Completed;
        EndedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = RoomStatus.Active;
        StartedAt = DateTime.UtcNow;
    }

    public void AddParticipant(RoomParticipant participant)
    {
        ArgumentNullException.ThrowIfNull(participant);
        _participants.Add(participant);
    }

    public void RemoveParticipant(RoomParticipant participant)
    {
        ArgumentNullException.ThrowIfNull(participant);
        _participants.Remove(participant);
    }

    public IReadOnlyCollection<RoomParticipant> GetParticipants() => Participants;
}
