using Torisho.Domain.Common;
using Torisho.Domain.Entities.ProgressDomain;
using Torisho.Domain.Enums;
using Torisho.Domain.Interfaces;

namespace Torisho.Domain.Entities.LearningDomain;

public sealed class Level : BaseEntity, IProgressable, IAggregateRoot
{
    public JLPTLevel Code { get; private set; }
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public int Order { get; private set; }
    public float RequiredProgressPercent { get; set; }
    public string? ThumbnailUrl { get; set; }

    // Non-aggregate references - EF Core navigation properties
    public ICollection<LearningProgress> LearningProgresses { get; private set; } = new List<LearningProgress>();
    
    // DDD: Aggregate - Level manages Chapters through domain methods
    private readonly HashSet<Chapter> _chapters = new();
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
        ArgumentNullException.ThrowIfNull(chapter);
        _chapters.Add(chapter);
    }

    public void RemoveChapter(Chapter chapter)
    {
        ArgumentNullException.ThrowIfNull(chapter);
        _chapters.Remove(chapter);
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
