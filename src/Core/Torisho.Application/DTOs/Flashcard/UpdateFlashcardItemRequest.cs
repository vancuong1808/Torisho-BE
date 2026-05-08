namespace Torisho.Application.DTOs.Flashcard;

public sealed record UpdateFlashcardItemRequest(
    string Front,
    string Back,
    string? Note = null,
    int? Position = null,
    bool? IsFavorite = null);
