using Torisho.Application.DTOs.Room;
using Torisho.Domain.Entities.RoomDomain;

namespace Torisho.Application.Mappers;

/// <summary>
/// Mapper for Room domain entities to DTOs
/// Follows Single Responsibility Principle - separates mapping logic from business logic
/// </summary>
public static class RoomMapper
{
    public static RoomDto ToDto(this Room room)
    {
        ArgumentNullException.ThrowIfNull(room);

        return new RoomDto
        {
            Id = room.Id,
            Status = room.Status,
            RoomType = room.RoomType,
            TargetLevel = room.TargetLevel,
            ScheduledAt = room.ScheduledAt,
            StartedAt = room.StartedAt,
            EndedAt = room.EndedAt,
            ParticipantCount = room.Participants.Count(p => !p.LeftAt.HasValue),
            MaxParticipants = room.MaxParticipants,
            Participants = room.Participants
                .Where(p => !p.LeftAt.HasValue)
                .Select(p => p.ToDto())
                .ToList()
        };
    }

    public static RoomParticipantDto ToDto(this RoomParticipant participant)
    {
        ArgumentNullException.ThrowIfNull(participant);

        return new RoomParticipantDto
        {
            UserId = participant.UserId,
            FullName = participant.User?.FullName ?? string.Empty,
            AvatarUrl = participant.User?.AvatarUrl,
            JoinedAt = participant.JoinedAt
        };
    }
}
