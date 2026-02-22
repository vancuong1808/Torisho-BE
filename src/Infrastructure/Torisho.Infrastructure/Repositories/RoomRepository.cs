using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Domain.Entities.RoomDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public class RoomRepository : GenericRepository<Room>, IRoomRepository
{
    public RoomRepository(IDataContext context) : base(context)
    {
    }

    public async Task<Room?> GetWithParticipantsAsync(Guid roomId, CancellationToken ct = default)
    {
        if (roomId == Guid.Empty)
            throw new ArgumentException("RoomId cannot be empty", nameof(roomId));

        return await _dbSet
            .Include(r => r.Participants)
                .ThenInclude(p => p.User) // Load User in4
            .FirstOrDefaultAsync(r => r.Id == roomId, ct);
    }
}