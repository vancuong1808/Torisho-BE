using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Domain.Entities.FlashcardDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public sealed class FlashcardItemRepository : IFlashcardItemRepository
{
    private readonly DbSet<FlashcardItem> _dbSet;

    public FlashcardItemRepository(IDataContext context)
    {
        _dbSet = context.Set<FlashcardItem>();
    }

    public async Task AddAsync(FlashcardItem entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
    }

    public async Task AddRangeAsync(IEnumerable<FlashcardItem> entities, CancellationToken ct = default)
    {
        await _dbSet.AddRangeAsync(entities, ct);
    }
}