using Torisho.Application.DTOs.Flashcard;

namespace Torisho.Application.Interfaces.Flashcard;

public interface IFlashcardStudyService
{
    Task<Guid> AddFromDictionaryAsync(Guid userId, Guid deckId, AddFromDictionaryRequest request, CancellationToken ct = default);
    Task UpdateItemAsync(Guid userId, Guid deckId, Guid itemId, UpdateFlashcardItemRequest request, CancellationToken ct = default);

    Task DeleteItemAsync(Guid userId, Guid deckId, Guid itemId, CancellationToken ct = default);

    Task<int> BulkImportAsync(Guid userId, Guid deckId, BulkImportRequest request, CancellationToken ct = default);
}