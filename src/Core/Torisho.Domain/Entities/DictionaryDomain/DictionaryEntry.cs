using Torisho.Domain.Common;
using Torisho.Domain.Entities.ContentDomain;
using Torisho.Domain.Enums;
using Torisho.Domain.Interfaces;

namespace Torisho.Domain.Entities.DictionaryDomain;

public sealed class DictionaryEntry : BaseEntity, IAggregateRoot, ISearchable
{
    // Stable id from JMdict (e.g., ent_seq) to support idempotent imports
    public string? SourceId { get; private set; }

    public string Keyword { get; private set; } = string.Empty;
    public string Reading { get; private set; } = string.Empty;
    public JLPTLevel Jlpt { get; private set; }
    public bool IsCommon { get; private set; }

    // Keep original JMdict JSON for display/debugging without heavy joins
    public string? RawJson { get; private set; }

    public string? MeaningsJson { get; private set; }
    public string? ExamplesJson { get; private set; }

    // Search/index helper tables (part of aggregate)
    public ICollection<DictionaryEntryKanjiForm> KanjiForms { get; private set; } = new List<DictionaryEntryKanjiForm>();
    public ICollection<DictionaryEntryReadingForm> ReadingForms { get; private set; } = new List<DictionaryEntryReadingForm>();
    public DictionaryEntryDefinition? Definition { get; private set; }

    // Non-aggregate references - EF Core navigation properties
    public ICollection<FlashCard> FlashCards { get; private set; } = new List<FlashCard>();
    public ICollection<Vocabulary> Vocabularies { get; private set; } = new List<Vocabulary>();
    public ICollection<Kanji> Kanjis { get; private set; } = new List<Kanji>();

    private DictionaryEntry() { }

    public DictionaryEntry(string keyword, string reading, JLPTLevel jlpt, string? meaningsJson = null,
        string? examplesJson = null, string? rawJson = null, string? sourceId = null, bool isCommon = false)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            throw new ArgumentException("Keyword is required", nameof(keyword));
        if (string.IsNullOrWhiteSpace(reading))
            throw new ArgumentException("Reading is required", nameof(reading));

        Keyword = keyword;
        Reading = reading;
        Jlpt = jlpt;
        RawJson = rawJson;
        SourceId = sourceId;
        IsCommon = isCommon;
        MeaningsJson = meaningsJson;
        ExamplesJson = examplesJson;
    }

    public void UpdatePrimaryForms(string keyword, string reading)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            throw new ArgumentException("Keyword is required", nameof(keyword));
        if (string.IsNullOrWhiteSpace(reading))
            throw new ArgumentException("Reading is required", nameof(reading));

        Keyword = keyword;
        Reading = reading;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetRawJson(string? rawJson)
    {
        RawJson = rawJson;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetIsCommon(bool isCommon)
    {
        IsCommon = isCommon;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReplaceKanjiForms(IEnumerable<(string Text, bool IsCommon)> forms)
    {
        KanjiForms.Clear();
        foreach (var (text, isCommon) in forms)
        {
            if (string.IsNullOrWhiteSpace(text))
                continue;
            KanjiForms.Add(new DictionaryEntryKanjiForm(Id, text, isCommon));
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReplaceReadingForms(IEnumerable<string> forms)
    {
        ReadingForms.Clear();
        foreach (var text in forms)
        {
            if (string.IsNullOrWhiteSpace(text))
                continue;
            ReadingForms.Add(new DictionaryEntryReadingForm(Id, text));
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetGlossText(string glossText)
    {
        if (string.IsNullOrWhiteSpace(glossText))
            throw new ArgumentException("GlossText is required", nameof(glossText));

        if (Definition is null)
            Definition = new DictionaryEntryDefinition(Id, glossText);
        else
            Definition.UpdateGlossText(glossText);
        UpdatedAt = DateTime.UtcNow;
    }

    public IEnumerable<object> Search(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return Enumerable.Empty<object>();

        var lowerKeyword = keyword.ToLower();
        if (Keyword.ToLower().Contains(lowerKeyword) ||
            Reading.ToLower().Contains(lowerKeyword))
        {
            return new[] { this };
        }
        return Enumerable.Empty<object>();
    }

    public IEnumerable<object> Filter(IDictionary<string, object> criteria)
    {
        if (criteria.TryGetValue("jlpt", out var jlptValue) && jlptValue is JLPTLevel jlptLevel)
        {
            if (Jlpt == jlptLevel)
                return new[] { this };
        }
        return Enumerable.Empty<object>();
    }
}
