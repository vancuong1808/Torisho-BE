using Torisho.Domain.Common;

namespace Torisho.Domain.Entities.LearningDomain;

public sealed class Chapter : BaseEntity
{
    private readonly HashSet<Lesson> _lessons = new();

    public Guid LevelId { get; private set; }
    public Level Level { get; private set; } = default!;

    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public int Order { get; private set; }
    public float RequiredProgressPercent { get; private set; }
    public string ThumbnailUrl { get; private set; } = default!;

    public IReadOnlyCollection<Lesson> Lessons => _lessons;

    private Chapter() { }

    public Chapter(Guid levelId, string title, string description, int order, float requiredProgressPercent, string thumbnailUrl)
    {
        LevelId = levelId;
        Title = title;
        Description = description;
        Order = order;
        RequiredProgressPercent = requiredProgressPercent;
        ThumbnailUrl = thumbnailUrl;
    }

    public void AddLesson(Lesson lesson)
    {
        _lessons.Add(lesson);
    }
}
