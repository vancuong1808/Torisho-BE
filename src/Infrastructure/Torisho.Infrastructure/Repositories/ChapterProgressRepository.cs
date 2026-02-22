using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Domain.Entities.ProgressDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public class ChapterProgressRepository : GenericRepository<ChapterProgress>, IChapterProgressRepository
{
    public ChapterProgressRepository(IDataContext context) : base(context)
    {
    }

    public async Task<ChapterProgress?> GetByUserAndChapterAsync(Guid userId, Guid chapterId, CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (chapterId == Guid.Empty)
            throw new ArgumentException("ChapterId cannot be empty", nameof(chapterId));

        return await _dbSet
            .FirstOrDefaultAsync(cp => 
                cp.UserId == userId && 
                cp.ChapterId == chapterId, ct);
    }

    public async Task<IEnumerable<ChapterProgress>> GetByUserAndLevelAsync(Guid userId, Guid levelId, CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (levelId == Guid.Empty)
            throw new ArgumentException("LevelId cannot be empty", nameof(levelId));

        // All chapters progress trong một level
        // Include Chapter để get chapter info
        return await _dbSet
            .AsNoTracking()
            .Include(cp => cp.Chapter)
            .Where(cp => cp.UserId == userId && cp.LevelId == levelId)
            .OrderBy(cp => cp.Chapter!.Order)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<ChapterProgress>> GetUnlockedChaptersAsync(Guid userId, CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        // Only unlocked chapters
        return await _dbSet
            .AsNoTracking()
            .Include(cp => cp.Chapter)
            .Where(cp => cp.UserId == userId && cp.IsUnlocked)
            .OrderByDescending(cp => cp.LastUpdated) // Most recent first
            .ToListAsync(ct);
    }
}