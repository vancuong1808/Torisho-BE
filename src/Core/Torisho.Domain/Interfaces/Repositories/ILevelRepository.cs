using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.LearningDomain;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Interfaces.Repositories;

public interface ILevelRepository : IRepository<Level>
{
    // Get level by JLPT code
    // Use cases: Direct level access by code (N5/N4/N3/N2/N1), Placement test result redirect, Deep linking
    Task<Level?> GetByCodeAsync(JLPTLevel code, CancellationToken ct = default);

    // Get level with chapters (eager loading)
    // Use cases: Display chapter list, Show learning path, Calculate level progress
    Task<Level?> GetWithChaptersAsync(Guid levelId, CancellationToken ct = default);

    // Get level with full structure: Level → Chapters → Lessons
    // Use cases: Full curriculum display, Export course outline, Admin bulk operations
    Task<Level?> GetWithFullStructureAsync(Guid levelId, CancellationToken ct = default);

    // Get all levels ordered by difficulty
    // Use cases: Level selection screen, Dashboard display, Navigation breadcrumb
    Task<IEnumerable<Level>> GetAllOrderedAsync(CancellationToken ct = default);
}
