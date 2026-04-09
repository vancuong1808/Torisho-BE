using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
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

    public async Task<IEnumerable<DictionaryEntry>> GetByJlptLevelAsync(JLPTLevel jlptLevel, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(de => de.Jlpt == jlptLevel)
            .OrderBy(de => de.Keyword)
            .ToListAsync(ct);
    }

}