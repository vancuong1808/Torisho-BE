using Torisho.Domain.Entities.FlashcardDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IFlashcardItemRepository
{
    Task AddAsync(FlashcardItem entity, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<FlashcardItem> entities, CancellationToken ct = default);
}