using System.Threading;
using System.Threading.Tasks;
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

    public async Task<Kanji?> GetByCharacterWithRelatedAsync(string character, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(character))
            return null;

        var normalized = character.Trim();

        return await _dbSet
            .AsNoTracking()
            .Include(k => k.DictionaryEntryLinks)
                .ThenInclude(link => link.DictionaryEntry)
            .AsSplitQuery()
            .FirstOrDefaultAsync(k => k.Character == normalized, ct);
    }
}
