using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Domain.Entities.FlashcardDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public sealed class FlashcardDeckRepository : GenericRepository<FlashcardDeck>, IFlashcardDeckRepository
{
    public FlashcardDeckRepository(IDataContext context) : base(context)
    {
    }

    public async Task<bool> IsOwnedByUserAsync(Guid deckId, Guid userId, CancellationToken ct = default)
    {
        if (deckId == Guid.Empty)
            throw new ArgumentException("DeckId cannot be empty", nameof(deckId));
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        return await _dbSet
            .AsNoTracking()
            .AnyAsync(deck => deck.Id == deckId && deck.UserId == userId, ct);
    }
}