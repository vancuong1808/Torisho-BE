using Torisho.Application.DTOs.Flashcard;

namespace Torisho.Application.Interfaces.Flashcard;

public interface IFlashcardQueryService
{
    Task<IEnumerable<FlashcardDeckSummaryDto>> GetUserDecksAsync(Guid userId, CancellationToken ct = default);
    Task<IEnumerable<FlashcardItemDto>> GetDeckItemsAsync(Guid deckId, Guid userId, CancellationToken ct = default);
}