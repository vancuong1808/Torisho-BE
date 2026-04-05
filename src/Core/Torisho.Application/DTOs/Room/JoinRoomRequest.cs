using Torisho.Domain.Enums;

namespace Torisho.Application.DTOs.Room;

public class JoinRoomRequest
{
    public JLPTLevel TargetLevel { get; set; }  // Level wanted to join
}