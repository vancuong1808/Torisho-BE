namespace Torisho.Domain.Entities.DictionaryDomain;

public sealed class DictionaryEntryDefinition
{
    public Guid DictionaryEntryId { get; private set; }
    public string GlossText { get; private set; } = string.Empty;

    private DictionaryEntryDefinition() { }

    public DictionaryEntryDefinition(Guid dictionaryEntryId, string glossText)
    {
        if (dictionaryEntryId == Guid.Empty)
            throw new ArgumentException("DictionaryEntryId cannot be empty", nameof(dictionaryEntryId));
        if (string.IsNullOrWhiteSpace(glossText))
            throw new ArgumentException("GlossText is required", nameof(glossText));

        DictionaryEntryId = dictionaryEntryId;
        GlossText = glossText;
    }

    public void UpdateGlossText(string glossText)
    {
        if (string.IsNullOrWhiteSpace(glossText))
            throw new ArgumentException("GlossText is required", nameof(glossText));

        GlossText = glossText;
    }
}
