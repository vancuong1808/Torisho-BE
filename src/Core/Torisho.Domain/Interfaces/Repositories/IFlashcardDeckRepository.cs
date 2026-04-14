using Torisho.Domain.Entities.FlashcardDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IFlashcardDeckRepository : IRepository<FlashcardDeck>
{
    Task<bool> IsOwnedByUserAsync(Guid deckId, Guid userId, CancellationToken ct = default);
}