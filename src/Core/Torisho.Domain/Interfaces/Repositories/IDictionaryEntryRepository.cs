using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.DictionaryDomain;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IDictionaryEntryRepository : IRepository<DictionaryEntry>
{
    // Get dictionary entry by id including relations needed for detail view
    Task<DictionaryEntry?> GetByIdWithRelationsAsync(Guid id, CancellationToken ct = default);
}
