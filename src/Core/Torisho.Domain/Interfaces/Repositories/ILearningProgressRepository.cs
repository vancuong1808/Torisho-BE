using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.ProgressDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface ILearningProgressRepository : IRepository<LearningProgress>
{
    Task<LearningProgress?> GetByUserAndLevelAsync(Guid userId, Guid levelId, CancellationToken ct = default);
    Task<IEnumerable<LearningProgress>> GetByUserAsync(Guid userId, CancellationToken ct = default);
}
