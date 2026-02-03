using Torisho.Domain.Common;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Domain.Entities.DictionaryDomain;

public sealed class FlashCard : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public Guid DictionaryEntryId { get; private set; }
    public DictionaryEntry DictionaryEntry { get; private set; } = default!;

    public string Front { get; private set; } = default!;
    public string Back { get; private set; } = default!;
    public bool IsFavorite { get; private set; }

    private FlashCard() { }

    public FlashCard(Guid userId, Guid dictionaryEntryId, string front, string back, bool isFavorite = false)
    {
        UserId = userId;
        DictionaryEntryId = dictionaryEntryId;
        Front = front;
        Back = back;
        IsFavorite = isFavorite;
    }

    public void Flip() { }
}
