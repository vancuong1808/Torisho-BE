using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.ProgressDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface ILearningProgressRepository : IRepository<LearningProgress>
{
    // Get learning progress for user and level</summary>
    // Use cases: Dashboard skill breakdown display, Certificate verification, Unlock next level check, Progress report
    Task<LearningProgress?> GetByUserAndLevelAsync(Guid userId, Guid levelId, CancellationToken ct = default);

    // Get all learning progress for user</summary>
    // Use cases: Profile overview across all levels, Overall statistics, Achievement tracking, Study recommendations
    Task<IEnumerable<LearningProgress>> GetByUserAsync(Guid userId, CancellationToken ct = default);
}
