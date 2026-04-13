using Torisho.Domain.Common;

namespace Torisho.Domain.Entities.LearningDomain;

public sealed class LessonVocabularyItem : BaseEntity
{
    public Guid LessonId { get; private set; }
    public Lesson? Lesson { get; private set; }

    public int SortOrder { get; private set; }
    public string Term { get; private set; } = string.Empty;
    public string Reading { get; private set; } = string.Empty;
    public string? Note { get; private set; }
    public string MeaningsJson { get; private set; } = "[]";
    public string? ExamplesJson { get; private set; }
    public string? OtherFormsJson { get; private set; }
    public bool IsCommon { get; private set; }
    public string? JlptTagsJson { get; private set; }

    private LessonVocabularyItem() { }

    public LessonVocabularyItem(
        Guid lessonId,
        int sortOrder,
        string term,
        string reading,
        string meaningsJson,
        string? note = null,
        string? examplesJson = null,
        string? otherFormsJson = null,
        bool isCommon = false,
        string? jlptTagsJson = null)
    {
        if (lessonId == Guid.Empty)
            throw new ArgumentException("LessonId cannot be empty", nameof(lessonId));
        if (sortOrder < 0)
            throw new ArgumentException("SortOrder must be non-negative", nameof(sortOrder));
        if (string.IsNullOrWhiteSpace(term))
            throw new ArgumentException("Term is required", nameof(term));
        if (string.IsNullOrWhiteSpace(reading))
            throw new ArgumentException("Reading is required", nameof(reading));
        if (string.IsNullOrWhiteSpace(meaningsJson))
            throw new ArgumentException("MeaningsJson is required", nameof(meaningsJson));

        LessonId = lessonId;
        SortOrder = sortOrder;
        Term = term;
        Reading = reading;
        MeaningsJson = meaningsJson;
        Note = note;
        ExamplesJson = examplesJson;
        OtherFormsJson = otherFormsJson;
        IsCommon = isCommon;
        JlptTagsJson = jlptTagsJson;
    }

    public void UpdatePayload(
        int sortOrder,
        string term,
        string reading,
        string meaningsJson,
        string? note,
        string? examplesJson,
        string? otherFormsJson,
        bool isCommon,
        string? jlptTagsJson)
    {
        if (sortOrder < 0)
            throw new ArgumentException("SortOrder must be non-negative", nameof(sortOrder));
        if (string.IsNullOrWhiteSpace(term))
            throw new ArgumentException("Term is required", nameof(term));
        if (string.IsNullOrWhiteSpace(reading))
            throw new ArgumentException("Reading is required", nameof(reading));
        if (string.IsNullOrWhiteSpace(meaningsJson))
            throw new ArgumentException("MeaningsJson is required", nameof(meaningsJson));

        SortOrder = sortOrder;
        Term = term;
        Reading = reading;
        MeaningsJson = meaningsJson;
        Note = note;
        ExamplesJson = examplesJson;
        OtherFormsJson = otherFormsJson;
        IsCommon = isCommon;
        JlptTagsJson = jlptTagsJson;
        UpdatedAt = DateTime.UtcNow;
    }
}