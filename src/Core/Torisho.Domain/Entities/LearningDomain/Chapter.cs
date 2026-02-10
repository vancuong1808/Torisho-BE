using Torisho.Domain.Common;

namespace Torisho.Domain.Entities.LearningDomain;

public sealed class Chapter : BaseEntity
{
    public Guid LevelId { get; private set; }
    public Level? Level { get; private set; }

    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int Order { get; private set; }
    public float RequiredProgressPercent { get; set; }
    public string? ThumbnailUrl { get; set; }
    
    // DDD: Aggregate - Chapter manages Lessons through domain methods
    private readonly HashSet<Lesson> _lessons = new();
    public IReadOnlyCollection<Lesson> Lessons => _lessons;

    private Chapter() { }

    public Chapter(Guid levelId, string title, string? description, int order, float requiredProgressPercent = 100f, string? thumbnailUrl = null)
    {
        if (levelId == Guid.Empty)
            throw new ArgumentException("LevelId cannot be empty", nameof(levelId));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required", nameof(title));
        if (order < 0)
            throw new ArgumentException("Order must be non-negative", nameof(order));
        if (requiredProgressPercent < 0 || requiredProgressPercent > 100)
            throw new ArgumentException("RequiredProgressPercent must be between 0 and 100", nameof(requiredProgressPercent));

        LevelId = levelId;
        Title = title;
        Description = description;
        Order = order;
        RequiredProgressPercent = requiredProgressPercent;
        ThumbnailUrl = thumbnailUrl;
    }

    public void AddLesson(Lesson lesson)
    {
        ArgumentNullException.ThrowIfNull(lesson);
        _lessons.Add(lesson);
    }

    public void RemoveLesson(Lesson lesson)
    {
        ArgumentNullException.ThrowIfNull(lesson);
        _lessons.Remove(lesson);
    }
}
