namespace Torisho.Application.DTOs.Flashcard;

public sealed record UpdateFlashcardDeckRequest(
    string Name,
    string? Description = null,
    Guid? FolderId = null);
