using Torisho.Domain.Common;
using Torisho.Domain.Entities.DictionaryDomain;

namespace Torisho.Domain.Entities.FlashcardDomain;

public sealed class FlashcardItem : BaseEntity
{
    public Guid DeckId { get; private set; }
    public FlashcardDeck? Deck { get; private set; }

    // Nullable to support bulk import cards that don't map to local dictionary entries.
    public Guid? DictionaryEntryId { get; private set; }
    public DictionaryEntry? DictionaryEntry { get; private set; }

    public string Front { get; private set; } = string.Empty;
    public string Back { get; private set; } = string.Empty;
    public string? Note { get; private set; }

    // search | bulk_import
    public string SourceType { get; private set; } = null!;
    public string? ExternalId { get; private set; }

    public bool IsFavorite { get; private set; }
    public int Position { get; private set; }

    private FlashcardItem() { }

    public FlashcardItem(
        Guid deckId,
        string front,
        string back,
        string sourceType,
        Guid? dictionaryEntryId = null,
        string? externalId = null,
        string? note = null,
        int position = 0,
        bool isFavorite = false)
    {
        if (deckId == Guid.Empty)
            throw new ArgumentException("DeckId cannot be empty", nameof(deckId));
        if (dictionaryEntryId.HasValue && dictionaryEntryId.Value == Guid.Empty)
            throw new ArgumentException("DictionaryEntryId cannot be empty Guid", nameof(dictionaryEntryId));
        if (string.IsNullOrWhiteSpace(front))
            throw new ArgumentException("Front is required", nameof(front));
        if (string.IsNullOrWhiteSpace(back))
            throw new ArgumentException("Back is required", nameof(back));
        if (string.IsNullOrWhiteSpace(sourceType))
            throw new ArgumentException("SourceType is required", nameof(sourceType));
        if (position < 0)
            throw new ArgumentException("Position cannot be negative", nameof(position));

        DeckId = deckId;
        DictionaryEntryId = dictionaryEntryId;
        Front = front.Trim();
        Back = back.Trim();
        Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        SourceType = sourceType.Trim().ToLowerInvariant();
        ExternalId = string.IsNullOrWhiteSpace(externalId) ? null : externalId.Trim();
        Position = position;
        IsFavorite = isFavorite;
    }

    public void UpdateContent(string front, string back, string? note = null)
    {
        if (string.IsNullOrWhiteSpace(front))
            throw new ArgumentException("Front is required", nameof(front));
        if (string.IsNullOrWhiteSpace(back))
            throw new ArgumentException("Back is required", nameof(back));

        Front = front.Trim();
        Back = back.Trim();
        Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDictionaryEntry(Guid? dictionaryEntryId)
    {
        if (dictionaryEntryId.HasValue && dictionaryEntryId.Value == Guid.Empty)
            throw new ArgumentException("DictionaryEntryId cannot be empty Guid", nameof(dictionaryEntryId));

        DictionaryEntryId = dictionaryEntryId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPosition(int position)
    {
        if (position < 0)
            throw new ArgumentException("Position cannot be negative", nameof(position));

        Position = position;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetFavorite(bool isFavorite)
    {
        IsFavorite = isFavorite;
        UpdatedAt = DateTime.UtcNow;
    }
}