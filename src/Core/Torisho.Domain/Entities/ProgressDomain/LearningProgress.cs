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

    public void UpdateSkillProgress(string skill, float value)
    {
        switch (skill.ToLower())
        {
            case "vocabulary":
                VocabularyProgress = value;
                break;
            case "grammar":
                GrammarProgress = value;
                break;
            case "reading":
                ReadingProgress = value;
                break;
            case "listening":
                ListeningProgress = value;
                break;
        }
        TotalProgress = CalculateTotalProgress();
        LastUpdated = DateTime.UtcNow;
    }

    public float CalculateTotalProgress()
    {
        return (VocabularyProgress + GrammarProgress + ReadingProgress + ListeningProgress) / 4f;
    }

    public void UpdateProgress(Guid userId, float progress)
    {
        if (UserId == userId)
        {
            TotalProgress = progress;
            LastUpdated = DateTime.UtcNow;
        }
    }

    public float CalculateProgress(Guid userId)
    {
        return UserId == userId ? TotalProgress : 0f;
    }

    public string GetStatus()
    {
        return TotalProgress switch
        {
            >= 100f => "Completed",
            >= 75f => "Advanced",
            >= 50f => "Intermediate",
            >= 25f => "Beginner",
            _ => "Started"
        };
    }
}
