using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.ProgressDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IDailyActivitiesRepository : IRepository<DailyActivities>
{
    // Get daily activities for user and date
    // Use cases: Display today's progress, Record new activities, Check daily goal completion, Streak tracking
    Task<DailyActivities?> GetByUserAndDateAsync(Guid userId, DateOnly date, CancellationToken ct = default);

    // Get daily activities for date range
    // Use cases: Weekly/Monthly reports, Activity heatmap, Streak calculation, Trend charts, Export data
    Task<IEnumerable<DailyActivities>> GetByUserAndRangeAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken ct = default);
}
