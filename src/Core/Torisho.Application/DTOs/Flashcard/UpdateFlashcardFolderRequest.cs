namespace Torisho.Application.DTOs.Flashcard;

public sealed record UpdateFlashcardFolderRequest(
    string Name,
    string? Description = null,
    int DisplayOrder = 0);
