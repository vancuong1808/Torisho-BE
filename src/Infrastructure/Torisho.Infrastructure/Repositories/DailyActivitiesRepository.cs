using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Domain.Entities.ProgressDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public class DailyActivitiesRepository : GenericRepository<DailyActivities>, IDailyActivitiesRepository
{
    public DailyActivitiesRepository(IDataContext context) : base(context)
    {
    }

    public async Task<DailyActivities?> GetByUserAndDateAsync(Guid userId, DateOnly date, CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        // Find activities cho specific date
        return await _dbSet
            .FirstOrDefaultAsync(da => 
                da.UserId == userId && 
                da.ActivityDate == date, ct);
    }

    public async Task<IEnumerable<DailyActivities>> GetByUserAndRangeAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (from > to)
            throw new ArgumentException("From date must be before To date");

        // Date range query
        return await _dbSet
            .AsNoTracking()
            .Where(da => 
                da.UserId == userId &&
                da.ActivityDate >= from &&
                da.ActivityDate <= to)
            .OrderBy(da => da.ActivityDate) 
            .ToListAsync(ct);
    }
}