using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.ProgressDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IDailyActivitiesRepository : IRepository<DailyActivities>
{
    Task<DailyActivities?> GetByUserAndDateAsync(Guid userId, DateOnly date, CancellationToken ct = default);
    Task<IEnumerable<DailyActivities>> GetByUserAndRangeAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken ct = default);
}
