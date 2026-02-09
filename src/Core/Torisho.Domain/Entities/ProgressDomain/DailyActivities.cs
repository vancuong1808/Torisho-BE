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

    public string? ActivityDetailsJson { get; private set; }

    private DailyActivities() { }

    public DailyActivities(Guid userId, DateOnly activityDate, string? activityDetailsJson = null)
    {
        UserId = userId;
        ActivityDate = activityDate;
        ActivityDetailsJson = activityDetailsJson;
    }

    public void RecordActivity(string activityType, int count = 1)
    {
        switch (activityType.ToLower())
        {
            case "vocabulary":
                VocabularyCount += count;
                break;
            case "grammar":
                GrammarCount += count;
                break;
            case "reading":
                ReadingCount += count;
                break;
            case "listening":
                ListeningCount += count;
                break;
            case "quiz":
                QuizCount += count;
                break;
            case "room":
                RoomCount += count;
                break;
            case "flashcard":
                FlashcardCount += count;
                break;
        }
        TotalPoints = CalculatePoints();
    }

    public int CalculatePoints()
    {
        return (VocabularyCount * 10) + 
               (GrammarCount * 10) + 
               (ReadingCount * 15) + 
               (ListeningCount * 15) + 
               (QuizCount * 20) + 
               (RoomCount * 30) + 
               (FlashcardCount * 5) + 
               (DailyChallengeCompleted ? 50 : 0);
    }

    public void CompleteDailyChallenge(float score)
    {
        DailyChallengeCompleted = true;
        DailyChallengeScore = score;
        TotalPoints = CalculatePoints();
    }

    public void AddMinutes(int minutes)
    {
        TotalMinutes += minutes;
    }
}
