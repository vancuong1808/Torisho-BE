using Torisho.Domain.Enums;

namespace Torisho.Application.DTOs.Room;

public class RoomDto
{
    public Guid Id { get; set; }
    public RoomStatus Status { get; set; }
    public RoomType RoomType { get; set; }
    public JLPTLevel? TargetLevel { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int ParticipantCount { get; set; }
    public int MaxParticipants { get; set; }
    public List<RoomParticipantDto> Participants { get; set; } = new();
}
