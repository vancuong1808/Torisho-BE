using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.DictionaryDomain;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IDictionaryEntryRepository : IRepository<DictionaryEntry>
{
    // Get dictionary entry by exact keyword
    Task<DictionaryEntry?> GetByKeywordAsync(string keyword, CancellationToken ct = default);

    // Search dictionary entries by keyword (partial match)
    Task<IEnumerable<DictionaryEntry>> SearchByKeywordAsync(string keyword, CancellationToken ct = default);

    // Get dictionary entries by JLPT level
    Task<IEnumerable<DictionaryEntry>> GetByJlptLevelAsync(JLPTLevel jlptLevel, CancellationToken ct = default);
}
