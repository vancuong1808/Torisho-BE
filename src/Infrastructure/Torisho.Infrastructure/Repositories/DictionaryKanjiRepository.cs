using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Domain.Entities.DictionaryDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public class DictionaryKanjiRepository : GenericRepository<Kanji>, IDictionaryKanjiRepository
{
    public DictionaryKanjiRepository(IDataContext context) : base(context)
    {
    }

    public async Task<(Kanji? KanjiInfo, IEnumerable<(Guid Id, string Keyword, string Reading)> RelatedEntries)> GetKanjiWithRelatedWordsAsync(string character, int limit = 10, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(character))
            return (null, Enumerable.Empty<(Guid, string, string)>());

        var normalized = character.Trim();

        var row = await _dbSet
            .AsNoTracking()
            .Where(k => k.Character == normalized)
            .Select(k => new
            {
                Kanji = k,
                Related = k.DictionaryEntryLinks
                    .OrderBy(x => x.Position)
                    .Select(x => new
                    {
                        x.DictionaryEntry!.Id,
                        x.DictionaryEntry!.Keyword,
                        x.DictionaryEntry!.Reading
                    })
                    .Take(limit)
                    .ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (row is null)
            return (null, Enumerable.Empty<(Guid, string, string)>());

        var relatedEntries = row.Related
            .Select(e => (e.Id, e.Keyword, e.Reading));

        return (row.Kanji, relatedEntries);
    }
}
