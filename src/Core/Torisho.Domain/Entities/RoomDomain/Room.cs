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
    public JLPTLevel? TargetLevel { get; private set;}
    public int MaxParticipants { get; private set; } = 2; // max 2 people in a room
    private Room() { }

    public Room(RoomType roomType, DateTime scheduledAt, Guid? aiCoachId = null, JLPTLevel? targetLevel = null)
    {
        RoomType = roomType;
        ScheduledAt = scheduledAt;
        AiCoachId = aiCoachId;
        TargetLevel = targetLevel;
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

    public bool IsFull() => Participants.Count(p => !p.LeftAt.HasValue) >= MaxParticipants;

    public bool CanJoin(Guid userId)
    {
        // Can join if:
        // 1. Room is waiting for players
        // 2. Room is not full (based on ACTIVE participants)
        // 3. User doesn't already have an ACTIVE participant in this room
        return Status == RoomStatus.Waiting
            && !IsFull()
            && !Participants.Any(p => p.UserId == userId && !p.LeftAt.HasValue);
    }

    public void SetTargetLevel(JLPTLevel level)
    {
        if (Status != RoomStatus.Waiting)
            throw new InvalidOperationException("Cannot change target level after the room has started.");
        TargetLevel = level;
    }
}
