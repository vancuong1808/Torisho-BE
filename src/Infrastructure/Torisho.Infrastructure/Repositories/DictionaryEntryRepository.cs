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

    public async Task<DictionaryEntry?> GetByIdWithRelationsAsync(Guid id, CancellationToken ct = default)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id is required", nameof(id));

        return await _dbSet
            .Include(e => e.KanjiForms)
            .Include(e => e.ReadingForms)
            .Include(e => e.Definition)
            .Include(e => e.KanjiLinks)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    

}