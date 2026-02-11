using System;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.RoomDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IRoomRepository : IRepository<Room>
{
    Task<Room?> GetWithParticipantsAsync(Guid roomId, CancellationToken ct = default);
}
