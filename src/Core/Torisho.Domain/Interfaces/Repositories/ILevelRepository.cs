using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.LearningDomain;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Interfaces.Repositories;

public interface ILevelRepository : IRepository<Level>
{
    Task<Level?> GetByCodeAsync(JLPTLevel code, CancellationToken ct = default);
    Task<Level?> GetWithChaptersAsync(Guid levelId, CancellationToken ct = default);
    Task<Level?> GetWithFullStructureAsync(Guid levelId, CancellationToken ct = default);
    Task<IEnumerable<Level>> GetAllOrderedAsync(CancellationToken ct = default);
}
