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

    public async Task<FlashcardItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, ct);
    }

    public async Task AddAsync(FlashcardItem entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
    }

    public async Task AddRangeAsync(IEnumerable<FlashcardItem> entities, CancellationToken ct = default)
    {
        await _dbSet.AddRangeAsync(entities, ct);
    }

    public Task UpdateAsync(FlashcardItem entity, CancellationToken ct = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(FlashcardItem entity, CancellationToken ct = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }
}