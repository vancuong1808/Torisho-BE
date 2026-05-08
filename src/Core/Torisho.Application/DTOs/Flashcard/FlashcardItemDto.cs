namespace Torisho.Application.DTOs.Flashcard;

public sealed record FlashcardItemDto
{
    public Guid Id { get; init; }
    public string Front { get; init; } = string.Empty;
    public string Back { get; init; } = string.Empty;
    public string SourceType { get; init; } = string.Empty;
    public bool IsFavorite { get; init; }
    public int Position { get; init; }
}