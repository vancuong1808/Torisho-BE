using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.DictionaryDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IDictionaryEntryRepository : IRepository<DictionaryEntry>
{
    // Get dictionary entry by exact keyword
    // Use cases: Word lookup, Vocabulary lesson display, Flashcard creation, Quick reference
    Task<DictionaryEntry?> GetByKeywordAsync(string keyword, CancellationToken ct = default);

    // Search dictionary entries by keyword (partial match)
    // Use cases: Search autocomplete, Dictionary search, Kanji lookup, Study list filtering
    Task<IEnumerable<DictionaryEntry>> SearchByKeywordAsync(string keyword, CancellationToken ct = default);
}
