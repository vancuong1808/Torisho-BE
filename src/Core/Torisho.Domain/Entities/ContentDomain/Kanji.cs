using System.Text.Json;
using Torisho.Domain.Common;
using Torisho.Domain.Entities.DictionaryDomain;
using Torisho.Domain.Entities.QuizDomain;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Entities.ContentDomain;

public sealed class Kanji : LearningContent, IAggregateRoot
{
    public string Character { get; private set; } = string.Empty;
    public string Onyomi { get; private set; } = string.Empty;
    public string Kunyomi { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public string MeaningsJson { get; private set; } = "[]";
    public int? JlptLevel { get; private set; }
    public int? Grade { get; private set; }
    public int StrokeCount { get; private set; }
    public int? Frequency { get; private set; }
    public string? UnicodeHex { get; private set; }

    public ICollection<DictionaryEntryKanji> DictionaryEntryLinks { get; private set; } = new List<DictionaryEntryKanji>();

    private Kanji() { }

    public Kanji(
        string title,
        Guid levelId,
        string character,
        string onyomi,
        string kunyomi,
        string type,
        string meaningsJson,
        int? jlptLevel,
        int? grade,
        int strokeCount,
        int? frequency,
        string? unicodeHex)
        : base(title, levelId)
    {
        if (string.IsNullOrWhiteSpace(character))
            throw new ArgumentException("Character is required", nameof(character));
        if (character.Trim().Length != 1)
            throw new ArgumentException("Character must contain exactly one kanji.", nameof(character));
        if (strokeCount < 0)
            throw new ArgumentException("StrokeCount cannot be negative", nameof(strokeCount));
        if (jlptLevel.HasValue && (jlptLevel.Value < 1 || jlptLevel.Value > 5))
            throw new ArgumentException("JlptLevel must be between 1 and 5", nameof(jlptLevel));
        if (grade.HasValue && grade.Value <= 0)
            throw new ArgumentException("Grade must be positive", nameof(grade));
        if (frequency.HasValue && frequency.Value <= 0)
            throw new ArgumentException("Frequency must be positive", nameof(frequency));

        var normalizedType = type.Trim().ToLowerInvariant();
        if (normalizedType is not ("" or "jouyou" or "jinmeiyou"))
            throw new ArgumentException("Type must be one of: '', 'jouyou', 'jinmeiyou'", nameof(type));

        Character = character.Trim();
        Onyomi = onyomi.Trim();
        Kunyomi = kunyomi.Trim();
        Type = normalizedType;
        MeaningsJson = NormalizeMeaningsJson(meaningsJson);
        JlptLevel = jlptLevel;
        Grade = grade;
        StrokeCount = strokeCount;
        Frequency = frequency;
        UnicodeHex = string.IsNullOrWhiteSpace(unicodeHex) ? null : unicodeHex.Trim().ToLowerInvariant();
    }

    public void UpdateMetadata(
        string onyomi,
        string kunyomi,
        string type,
        string meaningsJson,
        int? jlptLevel,
        int? grade,
        int strokeCount,
        int? frequency,
        string? unicodeHex)
    {
        if (strokeCount < 0)
            throw new ArgumentException("StrokeCount cannot be negative", nameof(strokeCount));
        if (jlptLevel.HasValue && (jlptLevel.Value < 1 || jlptLevel.Value > 5))
            throw new ArgumentException("JlptLevel must be between 1 and 5", nameof(jlptLevel));
        if (grade.HasValue && grade.Value <= 0)
            throw new ArgumentException("Grade must be positive", nameof(grade));
        if (frequency.HasValue && frequency.Value <= 0)
            throw new ArgumentException("Frequency must be positive", nameof(frequency));

        var normalizedType = type.Trim().ToLowerInvariant();
        if (normalizedType is not ("" or "jouyou" or "jinmeiyou"))
            throw new ArgumentException("Type must be one of: '', 'jouyou', 'jinmeiyou'", nameof(type));

        Onyomi = onyomi.Trim();
        Kunyomi = kunyomi.Trim();
        Type = normalizedType;
        MeaningsJson = NormalizeMeaningsJson(meaningsJson);
        JlptLevel = jlptLevel;
        Grade = grade;
        StrokeCount = strokeCount;
        Frequency = frequency;
        UnicodeHex = string.IsNullOrWhiteSpace(unicodeHex) ? null : unicodeHex.Trim().ToLowerInvariant();
        UpdatedAt = DateTime.UtcNow;
    }

    private static string NormalizeMeaningsJson(string meaningsJson)
    {
        if (string.IsNullOrWhiteSpace(meaningsJson))
            return "[]";

        try
        {
            using var doc = JsonDocument.Parse(meaningsJson);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                throw new ArgumentException("MeaningsJson must be a JSON array", nameof(meaningsJson));
        }
        catch (JsonException ex)
        {
            throw new ArgumentException("MeaningsJson must be valid JSON", nameof(meaningsJson), ex);
        }

        return meaningsJson;
    }

    public override void Display()
    {
        throw new NotImplementedException();
    }

    public override Quiz CreateQuiz()
        => new(QuizType.Kanji, Id);
}