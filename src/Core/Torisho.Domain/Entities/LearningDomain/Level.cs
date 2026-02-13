using Torisho.Domain.Common;
using Torisho.Domain.Entities.ProgressDomain;
using Torisho.Domain.Enums;
using Torisho.Domain.Interfaces;

namespace Torisho.Domain.Entities.LearningDomain;

public sealed class Level : BaseEntity, IProgressable, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int Order { get; private set; }
    public float RequiredProgressPercent { get; private set; }
    public string? ThumbnailUrl { get; private set; }
    public JLPTLevel Code { get; private set; }

    // DDD: Aggregate - Level manages Chapters through domain methods
    private readonly HashSet<Chapter> _chapters = new();
    public IReadOnlyCollection<Chapter> Chapters => _chapters;

    // Non-aggregate references - EF Core navigation properties
    public ICollection<LearningProgress> LearningProgresses { get; private set; } = new List<LearningProgress>();

    private Level() { }

    public Level(JLPTLevel code, string name, string? description, int order, float requiredProgressPercent = 100f, string? thumbnailUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        if (order < 0)
            throw new ArgumentException("Order must be non-negative", nameof(order));
        if (requiredProgressPercent < 0 || requiredProgressPercent > 100)
            throw new ArgumentException("RequiredProgressPercent must be between 0 and 100", nameof(requiredProgressPercent));

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
