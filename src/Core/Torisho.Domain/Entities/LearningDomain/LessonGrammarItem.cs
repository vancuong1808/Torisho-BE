using Torisho.Domain.Common;

namespace Torisho.Domain.Entities.LearningDomain;

public sealed class LessonGrammarItem : BaseEntity
{
    public Guid LessonId { get; private set; }
    public Lesson? Lesson { get; private set; }

    public int SortOrder { get; private set; }
    public string GrammarPoint { get; private set; } = string.Empty;
    public string MeaningEn { get; private set; } = string.Empty;
    public string? DetailUrl { get; private set; }
    public string? LevelHint { get; private set; }
    public string? Explanation { get; private set; }
    public string? UsageJson { get; private set; }
    public string? ExamplesJson { get; private set; }

    private LessonGrammarItem() { }

    public LessonGrammarItem(
        Guid lessonId,
        int sortOrder,
        string grammarPoint,
        string meaningEn,
        string? detailUrl = null,
        string? levelHint = null,
        string? explanation = null,
        string? usageJson = null,
        string? examplesJson = null)
    {
        if (lessonId == Guid.Empty)
            throw new ArgumentException("LessonId cannot be empty", nameof(lessonId));
        if (sortOrder < 0)
            throw new ArgumentException("SortOrder must be non-negative", nameof(sortOrder));
        if (string.IsNullOrWhiteSpace(grammarPoint))
            throw new ArgumentException("GrammarPoint is required", nameof(grammarPoint));

        LessonId = lessonId;
        SortOrder = sortOrder;
        GrammarPoint = grammarPoint;
        MeaningEn = meaningEn;
        DetailUrl = detailUrl;
        LevelHint = levelHint;
        Explanation = explanation;
        UsageJson = usageJson;
        ExamplesJson = examplesJson;
    }

    public void UpdatePayload(
        int sortOrder,
        string grammarPoint,
        string meaningEn,
        string? detailUrl,
        string? levelHint,
        string? explanation,
        string? usageJson,
        string? examplesJson)
    {
        if (sortOrder < 0)
            throw new ArgumentException("SortOrder must be non-negative", nameof(sortOrder));
        if (string.IsNullOrWhiteSpace(grammarPoint))
            throw new ArgumentException("GrammarPoint is required", nameof(grammarPoint));

        SortOrder = sortOrder;
        GrammarPoint = grammarPoint;
        MeaningEn = meaningEn;
        DetailUrl = detailUrl;
        LevelHint = levelHint;
        Explanation = explanation;
        UsageJson = usageJson;
        ExamplesJson = examplesJson;
        UpdatedAt = DateTime.UtcNow;
    }
}