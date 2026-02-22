using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Domain.Entities.LearningDomain;
using Torisho.Domain.Enums;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public class LevelRepository : GenericRepository<Level>, ILevelRepository
{
    public LevelRepository(IDataContext context) : base(context)
    {
    }

    public async Task<Level?> GetByCodeAsync(JLPTLevel code, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Code == code, ct);
    }

    public async Task<Level?> GetWithChaptersAsync(Guid levelId, CancellationToken ct = default)
    {
        if (levelId == Guid.Empty)
            throw new ArgumentException("LevelId cannot be empty", nameof(levelId));

        return await _dbSet
            .Include(l => l.Chapters.OrderBy(c => c.Order)) // Load Chapters và sort
            .FirstOrDefaultAsync(l => l.Id == levelId, ct);
    }

    public async Task<Level?> GetWithFullStructureAsync(Guid levelId, CancellationToken ct = default)
    {
        if (levelId == Guid.Empty)
            throw new ArgumentException("LevelId cannot be empty", nameof(levelId));

        return await _dbSet
            .Include(l => l.Chapters.OrderBy(c => c.Order))
                .ThenInclude(c => c.Lessons.OrderBy(lesson => lesson.Order)) 
            .FirstOrDefaultAsync(l => l.Id == levelId, ct);
    }

    public async Task<IEnumerable<Level>> GetAllOrderedAsync(CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .OrderBy(l => l.Order) 
            .ToListAsync(ct);
    }
}