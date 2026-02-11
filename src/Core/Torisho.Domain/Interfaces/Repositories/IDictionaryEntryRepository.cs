using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.DictionaryDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IDictionaryEntryRepository : IRepository<DictionaryEntry>
{
    Task<DictionaryEntry?> GetByKeywordAsync(string keyword, CancellationToken ct = default);
    Task<IEnumerable<DictionaryEntry>> SearchByKeywordAsync(string keyword, CancellationToken ct = default);
}
