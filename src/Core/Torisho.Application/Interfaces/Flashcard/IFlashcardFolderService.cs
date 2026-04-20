using Torisho.Application.DTOs.Flashcard;

namespace Torisho.Application.Interfaces.Flashcard;

public interface IFlashcardFolderService
{
    Task<Guid> CreateAsync(Guid userId, CreateFlashcardFolderRequest request, CancellationToken ct = default);

    Task UpdateAsync(Guid userId, Guid folderId, UpdateFlashcardFolderRequest request, CancellationToken ct = default);

    Task DeleteAsync(Guid userId, Guid folderId, CancellationToken ct = default);

    Task AddDeckAsync(Guid userId, Guid folderId, Guid deckId, CancellationToken ct = default);

    Task RemoveDeckAsync(Guid userId, Guid folderId, Guid deckId, CancellationToken ct = default);
}
