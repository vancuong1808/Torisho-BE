using Torisho.Domain.Common;
using Torisho.Domain.Entities.LearningDomain;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Domain.Entities.ProgressDomain;

public sealed class LearningProgress : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public User? User { get; private set; }

    public Guid LevelId { get; private set; }
    public Level? Level { get; private set; }

    public float VocabularyProgress { get; private set; }
    public float GrammarProgress { get; private set; }
    public float ReadingProgress { get; private set; }
    public float ListeningProgress { get; private set; }
    public float TotalProgress { get; private set; }

    // Snapshot of the lesson the user is currently studying in this level.
    public Guid? CurrentChapterId { get; private set; }
    public Guid? CurrentLessonId { get; private set; }
    public float CurrentLessonProgressPercent { get; private set; }
    public string? CurrentSection { get; private set; }

    public DateTime LastUpdated { get; private set; }

    private LearningProgress() { }

    public LearningProgress(Guid userId, Guid levelId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (levelId == Guid.Empty)
            throw new ArgumentException("LevelId cannot be empty", nameof(levelId));

        UserId = userId;
        LevelId = levelId;
        LastUpdated = DateTime.UtcNow;
    }

    public void SetCurrentLesson(Guid chapterId, Guid lessonId, float lessonProgressPercent, string? currentSection = null)
    {
        if (chapterId == Guid.Empty)
            throw new ArgumentException("ChapterId cannot be empty", nameof(chapterId));
        if (lessonId == Guid.Empty)
            throw new ArgumentException("LessonId cannot be empty", nameof(lessonId));
        if (lessonProgressPercent < 0 || lessonProgressPercent > 100)
            throw new ArgumentException("LessonProgressPercent must be between 0 and 100", nameof(lessonProgressPercent));

        CurrentChapterId = chapterId;
        CurrentLessonId = lessonId;
        CurrentLessonProgressPercent = lessonProgressPercent;
        CurrentSection = currentSection;
        LastUpdated = DateTime.UtcNow;
    }

    public void ClearCurrentLesson()
    {
        CurrentChapterId = null;
        CurrentLessonId = null;
        CurrentLessonProgressPercent = 0f;
        CurrentSection = null;
        LastUpdated = DateTime.UtcNow;
    }

    public void RefreshAggregates(
        float vocabularyProgress,
        float grammarProgress,
        float readingProgress,
        float listeningProgress,
        float totalProgress)
    {
        ValidatePercent(vocabularyProgress, nameof(vocabularyProgress));
        ValidatePercent(grammarProgress, nameof(grammarProgress));
        ValidatePercent(readingProgress, nameof(readingProgress));
        ValidatePercent(listeningProgress, nameof(listeningProgress));
        ValidatePercent(totalProgress, nameof(totalProgress));

        VocabularyProgress = vocabularyProgress;
        GrammarProgress = grammarProgress;
        ReadingProgress = readingProgress;
        ListeningProgress = listeningProgress;
        TotalProgress = totalProgress;
        LastUpdated = DateTime.UtcNow;
    }

    public string GetStatus()
    {
        return TotalProgress switch
        {
            >= 100f => "Completed",
            >= 75f => "Advanced",
            >= 50f => "Intermediate",
            >= 25f => "Beginner",
            > 0f => "Started",
            _ => "NotStarted"
        };
    }

    private static void ValidatePercent(float value, string name)
    {
        if (value < 0 || value > 100)
            throw new ArgumentException("Value must be between 0 and 100", name);
    }
}
