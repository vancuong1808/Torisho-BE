using Torisho.Application.DTOs.Flashcard;

namespace Torisho.Application.Interfaces.Flashcard;

public interface IFlashcardDeckService
{
    Task<Guid> CreateAsync(Guid userId, CreateFlashcardDeckRequest request, CancellationToken ct = default);

    Task UpdateAsync(Guid userId, Guid deckId, UpdateFlashcardDeckRequest request, CancellationToken ct = default);

    Task DeleteAsync(Guid userId, Guid deckId, CancellationToken ct = default);
}