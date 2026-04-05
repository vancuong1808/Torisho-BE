using System;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.RoomDomain;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IRoomRepository : IRepository<Room>
{
    // Get room with participants (eager loading)
    // Use cases: Display room lobby, Show active participants, Check room capacity, Admin participant management
    Task<Room?> GetWithParticipantsAsync(Guid roomId, CancellationToken ct = default);

    // find a waiting room by level (single)
    Task<Room?> FindWaitingRoomByLevelAsync(JLPTLevel level, CancellationToken ct = default);

    // find waiting rooms by level (multiple)
    Task<IEnumerable<Room>> FindWaitingRoomsByLevelAsync(JLPTLevel level, CancellationToken ct = default);

    // get the active room for a user
    Task<Room?> GetActiveRoomByUserIdAsync(Guid userId, CancellationToken ct = default);

    // check if a user is in an active room
    Task<bool> IsUserInActiveRoomAsync(Guid userId, CancellationToken ct = default);
}
