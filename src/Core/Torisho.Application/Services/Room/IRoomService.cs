using Torisho.Application.DTOs.Room;
using Torisho.Domain.Enums;

namespace Torisho.Application.Services.Room;

public interface IRoomService
{
    Task<RoomMatchResponse> FindOrCreateRoomAsync(Guid userId, JLPTLevel targetLevel, CancellationToken ct = default);
    Task<RoomDto> JoinRoomAsync(Guid userId, Guid roomId, CancellationToken ct = default);
    Task LeaveRoomAsync(Guid userId, Guid roomId, CancellationToken ct = default);
    Task<RoomDto> StartRoomAsync(Guid roomId, CancellationToken ct = default);
    Task<RoomDto> EndRoomAsync(Guid roomId, CancellationToken ct = default);
    Task<RoomDto?> GetCurrentUserRoomAsync(Guid userId, CancellationToken ct = default);
    Task<RoomDto?> GetRoomByIdAsync(Guid roomId, CancellationToken ct = default);
}