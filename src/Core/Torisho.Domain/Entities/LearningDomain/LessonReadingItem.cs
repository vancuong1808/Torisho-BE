using Torisho.Domain.Common;

namespace Torisho.Domain.Entities.LearningDomain;

public sealed class LessonReadingItem : BaseEntity
{
    public Guid LessonId { get; private set; }
    public Lesson? Lesson { get; private set; }

    public int SortOrder { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public string? Translation { get; private set; }
    public string? Url { get; private set; }
    public string? LevelHint { get; private set; }
    public string? Source { get; private set; }

    private LessonReadingItem() { }

    public LessonReadingItem(
        Guid lessonId,
        int sortOrder,
        string title,
        string content,
        string? translation = null,
        string? url = null,
        string? levelHint = null,
        string? source = null)
    {
        if (lessonId == Guid.Empty)
            throw new ArgumentException("LessonId cannot be empty", nameof(lessonId));
        if (sortOrder < 0)
            throw new ArgumentException("SortOrder must be non-negative", nameof(sortOrder));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required", nameof(title));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required", nameof(content));

        LessonId = lessonId;
        SortOrder = sortOrder;
        Title = title;
        Content = content;
        Translation = translation;
        Url = url;
        LevelHint = levelHint;
        Source = source;
    }

    public void UpdatePayload(
        int sortOrder,
        string title,
        string content,
        string? translation,
        string? url,
        string? levelHint,
        string? source)
    {
        if (sortOrder < 0)
            throw new ArgumentException("SortOrder must be non-negative", nameof(sortOrder));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required", nameof(title));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required", nameof(content));

        SortOrder = sortOrder;
        Title = title;
        Content = content;
        Translation = translation;
        Url = url;
        LevelHint = levelHint;
        Source = source;
        UpdatedAt = DateTime.UtcNow;
    }
}