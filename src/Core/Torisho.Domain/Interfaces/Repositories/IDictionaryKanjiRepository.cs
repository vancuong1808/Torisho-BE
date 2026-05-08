using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.DictionaryDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IDictionaryKanjiRepository : IRepository<Kanji>
{
    Task<(Kanji? KanjiInfo, IEnumerable<(Guid Id, string Keyword, string Reading)> RelatedEntries)> GetKanjiWithRelatedWordsAsync(string character, int limit = 10, CancellationToken ct = default);
}