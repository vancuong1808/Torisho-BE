using System;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.RoomDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IRoomRepository : IRepository<Room>
{
    // Get room with participants (eager loading)
    // Use cases: Display room lobby, Show active participants, Check room capacity, Admin participant management
    Task<Room?> GetWithParticipantsAsync(Guid roomId, CancellationToken ct = default);
}
