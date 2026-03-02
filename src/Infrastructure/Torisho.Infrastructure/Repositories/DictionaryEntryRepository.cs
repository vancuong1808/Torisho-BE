using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Domain.Entities.DictionaryDomain;
using Torisho.Domain.Enums;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public class DictionaryEntryRepository : GenericRepository<DictionaryEntry>, IDictionaryEntryRepository
{
    public DictionaryEntryRepository(IDataContext context) : base(context)
    {
    }

    public async Task<DictionaryEntry?> GetByKeywordAsync(string keyword, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            throw new ArgumentException("Keyword cannot be empty", nameof(keyword));

        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(de => de.Keyword == keyword, ct);
    }

    public async Task<IEnumerable<DictionaryEntry>> SearchByKeywordAsync(string keyword, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return Enumerable.Empty<DictionaryEntry>();

        return await _dbSet
            .AsNoTracking()
            .Where(de => de.Keyword.Contains(keyword) || de.Reading.Contains(keyword))
            .Take(50) // Limit results cho performance
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<DictionaryEntry>> GetByJlptLevelAsync(JLPTLevel jlptLevel, CancellationToken ct = default)
    {
       throw new NotImplementedException();
    }

}