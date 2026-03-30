namespace Torisho.Domain.Entities.DictionaryDomain;

public sealed class DictionaryEntryKanjiForm
{
    public Guid DictionaryEntryId { get; private set; }
    public string KanjiText { get; private set; } = string.Empty;
    public bool IsCommon { get; private set; }

    private DictionaryEntryKanjiForm() { }

    public DictionaryEntryKanjiForm(Guid dictionaryEntryId, string kanjiText, bool isCommon)
    {
        if (dictionaryEntryId == Guid.Empty)
            throw new ArgumentException("DictionaryEntryId cannot be empty", nameof(dictionaryEntryId));
        if (string.IsNullOrWhiteSpace(kanjiText))
            throw new ArgumentException("KanjiText is required", nameof(kanjiText));

        DictionaryEntryId = dictionaryEntryId;
        KanjiText = kanjiText;
        IsCommon = isCommon;
    }
}
