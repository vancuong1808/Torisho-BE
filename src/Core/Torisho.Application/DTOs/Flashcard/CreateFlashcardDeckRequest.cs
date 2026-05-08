namespace Torisho.Application.DTOs.Flashcard;

public sealed class CreateFlashcardDeckRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? FolderId { get; set; }
}