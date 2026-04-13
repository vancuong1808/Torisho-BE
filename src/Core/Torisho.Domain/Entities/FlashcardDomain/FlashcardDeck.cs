using Torisho.Domain.Common;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Domain.Entities.FlashcardDomain;

public sealed class FlashcardDeck : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public User? User { get; private set; }

    public Guid? FolderId { get; private set; }
    public FlashcardFolder? Folder { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    // Optional metadata for bulk import sources (Quizlet, CSV, etc.)
    public string? ImportSource { get; private set; }
    public string? ImportReference { get; private set; }

    public bool IsArchived { get; private set; }

    public ICollection<FlashcardItem> Items { get; private set; } = new List<FlashcardItem>();

    private FlashcardDeck() { }

    public FlashcardDeck(
        Guid userId,
        string name,
        Guid? folderId = null,
        string? description = null,
        string? importSource = null,
        string? importReference = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (folderId.HasValue && folderId.Value == Guid.Empty)
            throw new ArgumentException("FolderId cannot be empty Guid", nameof(folderId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Deck name is required", nameof(name));

        UserId = userId;
        FolderId = folderId;
        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        ImportSource = string.IsNullOrWhiteSpace(importSource) ? null : importSource.Trim().ToLowerInvariant();
        ImportReference = string.IsNullOrWhiteSpace(importReference) ? null : importReference.Trim();
    }

    public void SetFolder(Guid? folderId)
    {
        if (folderId.HasValue && folderId.Value == Guid.Empty)
            throw new ArgumentException("FolderId cannot be empty Guid", nameof(folderId));

        FolderId = folderId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Deck name is required", nameof(name));

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkImported(string importSource, string? importReference = null)
    {
        if (string.IsNullOrWhiteSpace(importSource))
            throw new ArgumentException("ImportSource is required", nameof(importSource));

        ImportSource = importSource.Trim().ToLowerInvariant();
        ImportReference = string.IsNullOrWhiteSpace(importReference) ? null : importReference.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetArchived(bool isArchived)
    {
        IsArchived = isArchived;
        UpdatedAt = DateTime.UtcNow;
    }
}