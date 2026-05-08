namespace Torisho.Application.DTOs.Flashcard;

public sealed record CreateFlashcardFolderRequest(
    string Name,
    string? Description = null,
    int DisplayOrder = 0);
