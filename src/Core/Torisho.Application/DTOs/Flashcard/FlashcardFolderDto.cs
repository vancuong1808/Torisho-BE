namespace Torisho.Application.DTOs.Flashcard;

public sealed record FlashcardFolderDto(
    Guid Id,
    string Name,
    string? Description,
    int DisplayOrder,
    int TotalDecks,
    DateTime CreatedAt);
