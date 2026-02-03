using Torisho.Domain.Common;
using Torisho.Domain.Entities.LearningDomain;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Domain.Entities.ProgressDomain;

public sealed class LearningProgress : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public Guid LevelId { get; private set; }
    public Level Level { get; private set; } = default!;

    public float VocabularyProgress { get; private set; }
    public float GrammarProgress { get; private set; }
    public float ReadingProgress { get; private set; }
    public float ListeningProgress { get; private set; }
    public float TotalProgress { get; private set; }
    public DateTime LastUpdated { get; private set; }

    private LearningProgress() { }

    public LearningProgress(Guid userId, Guid levelId)
    {
        UserId = userId;
        LevelId = levelId;
        LastUpdated = DateTime.UtcNow;
    }
}
