using Torisho.Domain.Enums;

namespace Torisho.Application.DTOs.Room;

public class RoomMatchedDto
{
    public Guid RoomId { get; set; }
    public RoomStatus Status { get; set; }
    public int ParticipantCount { get; set; }
    public string Message { get; set; } = string.Empty;
}
