using Torisho.Domain.Common;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Entities.LearningDomain;

public sealed class Level : BaseEntity, IAggregateRoot
{
    private readonly HashSet<Chapter> _chapters = new();

    public JLPTLevel Code { get; private set; }
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public int Order { get; private set; }
    public float RequiredProgressPercent { get; private set; }
    public string ThumbnailUrl { get; private set; } = default!;

    public IReadOnlyCollection<Chapter> Chapters => _chapters;

    private Level() { }

    public Level(JLPTLevel code, string name, string description, int order, float requiredProgressPercent, string thumbnailUrl)
    {
        Code = code;
        Name = name;
        Description = description;
        Order = order;
        RequiredProgressPercent = requiredProgressPercent;
        ThumbnailUrl = thumbnailUrl;
    }

    public void AddChapter(Chapter chapter)
    {
        _chapters.Add(chapter);
    }

    public float CalculateProgress(Guid userId)
    {
        throw new NotImplementedException();
    }

    public void UpdateProgress(Guid userId, float progress)
    {
        throw new NotImplementedException();
    }
}
