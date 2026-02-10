using Torisho.Domain.Common;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Domain.Entities.DictionaryDomain;

public sealed class FlashCard : BaseEntity
{
    public Guid UserId { get; private set; }
    public User? User { get; private set; }

    public Guid DictionaryEntryId { get; private set; }
    public DictionaryEntry? DictionaryEntry { get; private set; }

    public string Front { get; private set; } = string.Empty;
    public string Back { get; private set; } = string.Empty;
    public bool IsFavorite { get; private set; }

    private FlashCard() { }

    public FlashCard(Guid userId, Guid dictionaryEntryId, string front, string back, bool isFavorite = false)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (dictionaryEntryId == Guid.Empty)
            throw new ArgumentException("DictionaryEntryId cannot be empty", nameof(dictionaryEntryId));
        if (string.IsNullOrWhiteSpace(front))
            throw new ArgumentException("Front is required", nameof(front));
        if (string.IsNullOrWhiteSpace(back))
            throw new ArgumentException("Back is required", nameof(back));

        UserId = userId;
        DictionaryEntryId = dictionaryEntryId;
        Front = front;
        Back = back;
        IsFavorite = isFavorite;
    }

    public void Flip() { }
}
