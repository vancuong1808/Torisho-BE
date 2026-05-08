using Torisho.Domain.Entities.FlashcardDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IFlashcardItemRepository
{
    Task<FlashcardItem?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task AddAsync(FlashcardItem entity, CancellationToken ct = default);

    Task AddRangeAsync(IEnumerable<FlashcardItem> entities, CancellationToken ct = default);

    Task UpdateAsync(FlashcardItem entity, CancellationToken ct = default);

    Task DeleteAsync(FlashcardItem entity, CancellationToken ct = default);
}