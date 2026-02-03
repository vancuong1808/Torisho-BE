using Torisho.Domain.Common;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Domain.Entities.ProgressDomain;

public sealed class DailyActivities : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public DateOnly ActivityDate { get; private set; }

    public int VocabularyCount { get; private set; }
    public int GrammarCount { get; private set; }
    public int ReadingCount { get; private set; }
    public int ListeningCount { get; private set; }
    public int QuizCount { get; private set; }
    public int RoomCount { get; private set; }
    public int FlashcardCount { get; private set; }

    public bool DailyChallengeCompleted { get; private set; }
    public float? DailyChallengeScore { get; private set; }

    public int TotalMinutes { get; private set; }
    public int TotalPoints { get; private set; }

    public string ActivityDetailsJson { get; private set; } = default!;

    private DailyActivities() { }

    public DailyActivities(Guid userId, DateOnly activityDate, string activityDetailsJson)
    {
        UserId = userId;
        ActivityDate = activityDate;
        ActivityDetailsJson = activityDetailsJson;
    }
}
