using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Domain.Entities.ProgressDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public class LearningProgressRepository : GenericRepository<LearningProgress>, ILearningProgressRepository
{
    public LearningProgressRepository(IDataContext context) : base(context)
    {
    }

    public async Task<LearningProgress?> GetByUserAndLevelAsync(Guid userId, Guid levelId, CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (levelId == Guid.Empty)
            throw new ArgumentException("LevelId cannot be empty", nameof(levelId));

        // Find progress của user cho specific level
        return await _dbSet
            .FirstOrDefaultAsync(lp => 
                lp.UserId == userId && 
                lp.LevelId == levelId, ct);
    }

    public async Task<IEnumerable<LearningProgress>> GetByUserAsync(Guid userId, CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        // All progress của user across levels
        return await _dbSet
            .AsNoTracking()
            .Include(lp => lp.Level)
            .Where(lp => lp.UserId == userId)
            .ToListAsync(ct);
    }
}