namespace Torisho.Application.DTOs.Flashcard;

public sealed record FlashcardDeckSummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid? FolderId { get; init; }
    public string? FolderName { get; init; }
    public int TotalItems { get; init; }
    public DateTime CreatedAt { get; init; }
}