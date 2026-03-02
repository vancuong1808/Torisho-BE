namespace Torisho.Domain.Entities.DictionaryDomain;

public sealed class DictionaryEntryReadingForm
{
    public Guid DictionaryEntryId { get; private set; }
    public string ReadingText { get; private set; } = string.Empty;

    private DictionaryEntryReadingForm() { }

    public DictionaryEntryReadingForm(Guid dictionaryEntryId, string readingText)
    {
        if (dictionaryEntryId == Guid.Empty)
            throw new ArgumentException("DictionaryEntryId cannot be empty", nameof(dictionaryEntryId));
        if (string.IsNullOrWhiteSpace(readingText))
            throw new ArgumentException("ReadingText is required", nameof(readingText));

        DictionaryEntryId = dictionaryEntryId;
        ReadingText = readingText;
    }
}
