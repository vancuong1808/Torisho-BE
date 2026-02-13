using Torisho.Domain.Common;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Domain.Entities.QuizDomain;

public sealed class QuizAttempt : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public User? User { get; private set; }
    public Guid QuizId { get; private set; }
    public Quiz? Quiz { get; private set; }
    public float Score { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    // DDD: Aggregate - QuizAttempt manages Answers through domain methods
    private readonly HashSet<QuizAnswer> _answers = new();
    public IReadOnlyCollection<QuizAnswer> Answers => _answers;

    private QuizAttempt() { }

    public QuizAttempt(Guid userId, Guid quizId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (quizId == Guid.Empty)
            throw new ArgumentException("QuizId cannot be empty", nameof(quizId));

        UserId = userId;
        QuizId = quizId;
        StartedAt = DateTime.UtcNow;
    }

    public void Submit()
    {
        CompletedAt = DateTime.UtcNow;
        Score = CalculateScore();
    }

    public float CalculateScore()
    {
        if (!_answers.Any()) return 0f;
        var correct = _answers.Count(a => a.IsCorrect);
        return (float)correct / _answers.Count * 100f;
    }

    public IDictionary<string, object?> GetResult()
    {
        return new Dictionary<string, object?>
        {
            { "score", Score },
            { "startedAt", StartedAt },
            { "completedAt", CompletedAt },
            { "totalQuestions", _answers.Count },
            { "correctAnswers", _answers.Count(a => a.IsCorrect) }
        };
    }

    public void AddAnswer(QuizAnswer answer)
    {
        ArgumentNullException.ThrowIfNull(answer);
        
        // Business rule: không answer 2 lần cùng question
        if (_answers.Any(a => a.QuestionId == answer.QuestionId))
            throw new InvalidOperationException("Question already answered");
            
        _answers.Add(answer);
    }
}
