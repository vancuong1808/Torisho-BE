using Torisho.Domain.Entities.ContentDomain;

namespace Torisho.Domain.Entities.DictionaryDomain;

public sealed class DictionaryEntryKanji
{
    public Guid DictionaryEntryId { get; private set; }
    public DictionaryEntry? DictionaryEntry { get; private set; }

    public Guid KanjiId { get; private set; }
    public Kanji? Kanji { get; private set; }

    public int Position { get; private set; }

    private DictionaryEntryKanji() { }

    public DictionaryEntryKanji(Guid dictionaryEntryId, Guid kanjiId, int position = 0)
    {
        if (dictionaryEntryId == Guid.Empty)
            throw new ArgumentException("DictionaryEntryId cannot be empty", nameof(dictionaryEntryId));
        if (kanjiId == Guid.Empty)
            throw new ArgumentException("KanjiId cannot be empty", nameof(kanjiId));
        if (position < 0)
            throw new ArgumentException("Position cannot be negative", nameof(position));

        DictionaryEntryId = dictionaryEntryId;
        KanjiId = kanjiId;
        Position = position;
    }
}
