using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Domain.Entities.RoomDomain;
using Torisho.Domain.Enums;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public class RoomRepository : GenericRepository<Room>, IRoomRepository
{
    private const int WaitingRoomMatchWindowMinutes = 30;

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

    public async Task<Room?> FindWaitingRoomByLevelAsync(JLPTLevel level, CancellationToken ct = default)
    {
        var cutoff = DateTime.UtcNow.AddMinutes(-WaitingRoomMatchWindowMinutes);

        return await _dbSet
            .Include(r => r.Participants)
                .ThenInclude(p => p.User)
            .Where(r => r.Status == RoomStatus.Waiting
                 && r.RoomType == RoomType.UserToUser
                 && r.TargetLevel == level
                 && r.CreatedAt >= cutoff
                 && r.Participants.Count(p => !p.LeftAt.HasValue) < r.MaxParticipants)
            .OrderBy(r => r.CreatedAt)  // FIFO 
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<Room>> FindWaitingRoomsByLevelAsync(JLPTLevel level, CancellationToken ct = default)
    {
        var cutoff = DateTime.UtcNow.AddMinutes(-WaitingRoomMatchWindowMinutes);

        return await _dbSet
            .Include(r => r.Participants)
                .ThenInclude(p => p.User)
            .Where(r => r.Status == RoomStatus.Waiting
                 && r.RoomType == RoomType.UserToUser
                 && r.TargetLevel == level
                 && r.CreatedAt >= cutoff
                 && r.Participants.Count(p => !p.LeftAt.HasValue) < r.MaxParticipants)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<Room?> GetActiveRoomByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        return await _dbSet
            .Include(r => r.Participants)
                .ThenInclude(p => p.User)
            .Where(r => (r.Status == RoomStatus.Waiting || r.Status == RoomStatus.Active)
                 && r.Participants.Any(p => p.UserId == userId && !p.LeftAt.HasValue))
            .OrderByDescending(r => r.CreatedAt) // Get the most recent one
            .FirstOrDefaultAsync(ct);
    }

    public async Task<bool> IsUserInActiveRoomAsync(Guid userId, CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        return await _dbSet
            .AnyAsync(r => (r.Status == RoomStatus.Waiting || r.Status == RoomStatus.Active)
                    && r.Participants.Any(p => p.UserId == userId && !p.LeftAt.HasValue), ct);
    }
}