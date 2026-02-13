using Torisho.Domain.Common;
using Torisho.Domain.Entities.LearningDomain;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Domain.Entities.ProgressDomain;

public sealed class ChapterProgress : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public User? User { get; private set; }
    
    public Guid ChapterId { get; private set; }
    public Chapter? Chapter { get; private set; }
    
    public Guid LevelId { get; private set; }
    
    public bool IsUnlocked { get; private set; }
    public int CompletedLessonCount { get; private set; }
    public int TotalLessonCount { get; private set; }
    public float CompletionPercent { get; private set; }
    
    public DateTime? UnlockedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime LastUpdated { get; private set; }

    // DDD: Aggregate - ChapterProgress manages CompletedLessonIds through domain methods
    private readonly HashSet<Guid> _completedLessonIds = new();
    public IReadOnlyCollection<Guid> CompletedLessonIds => _completedLessonIds;

    private ChapterProgress() { }

    public ChapterProgress(Guid userId, Guid chapterId, Guid levelId, int totalLessonCount, bool isUnlocked = false)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (chapterId == Guid.Empty)
            throw new ArgumentException("ChapterId cannot be empty", nameof(chapterId));
        if (levelId == Guid.Empty)
            throw new ArgumentException("LevelId cannot be empty", nameof(levelId));
        if (totalLessonCount <= 0)
            throw new ArgumentException("TotalLessonCount must be positive", nameof(totalLessonCount));

        UserId = userId;
        ChapterId = chapterId;
        LevelId = levelId;
        TotalLessonCount = totalLessonCount;
        IsUnlocked = isUnlocked;
        LastUpdated = DateTime.UtcNow;
        
        if (isUnlocked)
            UnlockedAt = DateTime.UtcNow;
    }

    public void Unlock()
    {
        if (!IsUnlocked)
        {
            IsUnlocked = true;
            UnlockedAt = DateTime.UtcNow;
            LastUpdated = DateTime.UtcNow;
        }
    }

    public void IncrementCompletedLesson()
    {
        if (CompletedLessonCount < TotalLessonCount)
        {
            CompletedLessonCount++;
            RecalculateProgress();
        }
    }

    public void RecalculateProgress()
    {
        CompletionPercent = TotalLessonCount > 0 
            ? (float)CompletedLessonCount / TotalLessonCount * 100f 
            : 0f;

        if (CompletionPercent >= 100f && !CompletedAt.HasValue)
        {
            CompletedAt = DateTime.UtcNow;
        }

        LastUpdated = DateTime.UtcNow;
    }

    public bool IsCompleted() => CompletionPercent >= 100f;
}