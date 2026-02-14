using Torisho.Domain.Common;
using Torisho.Domain.Entities.ContentDomain;
using Torisho.Domain.Enums;
using Torisho.Domain.Interfaces;

namespace Torisho.Domain.Entities.DictionaryDomain;

public sealed class DictionaryEntry : BaseEntity, IAggregateRoot, ISearchable
{
    public string Keyword { get; private set; } = string.Empty;
    public string Reading { get; private set; } = string.Empty;
    public JLPTLevel Jlpt { get; private set; }
    public string? MeaningsJson { get; private set; }
    public string? ExamplesJson { get; private set; }

    // Non-aggregate references - EF Core navigation properties
    public ICollection<FlashCard> FlashCards { get; private set; } = new List<FlashCard>();
    public ICollection<Vocabulary> Vocabularies { get; private set; } = new List<Vocabulary>();
    public ICollection<Kanji> Kanjis { get; private set; } = new List<Kanji>();

    private DictionaryEntry() { }

    public DictionaryEntry(string keyword, string reading, JLPTLevel jlpt, string? meaningsJson = null,
        string? examplesJson = null)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            throw new ArgumentException("Keyword is required", nameof(keyword));
        if (string.IsNullOrWhiteSpace(reading))
            throw new ArgumentException("Reading is required", nameof(reading));

        Keyword = keyword;
        Reading = reading;
        Jlpt = jlpt;
        MeaningsJson = meaningsJson;
        ExamplesJson = examplesJson;
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
