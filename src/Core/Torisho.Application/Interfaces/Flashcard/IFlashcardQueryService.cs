using Torisho.Application.DTOs.Flashcard;

namespace Torisho.Application.Interfaces.Flashcard;

public interface IFlashcardQueryService
{
    Task<IEnumerable<FlashcardFolderDto>> GetUserFoldersAsync(Guid userId, CancellationToken ct = default);

    Task<FlashcardDeckDto?> GetDeckByIdAsync(Guid userId, Guid deckId, CancellationToken ct = default);

    Task<IEnumerable<FlashcardDeckDto>> GetUserDecksAsync(
        Guid userId,
        Guid? folderId = null,
        CancellationToken ct = default);

    Task<IEnumerable<FlashcardItemDto>> GetDeckItemsAsync(Guid deckId, Guid userId, CancellationToken ct = default);
}