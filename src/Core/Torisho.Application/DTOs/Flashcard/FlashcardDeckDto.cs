namespace Torisho.Application.DTOs.Flashcard;

public sealed record FlashcardDeckDto(
    Guid Id,
    string Name,
    string? Description,
    Guid? FolderId,
    string? FolderName,
    int TotalItems,
    DateTime CreatedAt);