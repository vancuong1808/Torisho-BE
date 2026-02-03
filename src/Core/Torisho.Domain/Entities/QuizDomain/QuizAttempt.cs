using Torisho.Domain.Common;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Domain.Entities.QuizDomain;

public sealed class QuizAttempt : BaseEntity, IAggregateRoot
{
    private readonly HashSet<QuizAnswer> _answers = new();

    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public Guid QuizId { get; private set; }
    public Quiz Quiz { get; private set; } = default!;

    public float Score { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public IReadOnlyCollection<QuizAnswer> Answers => _answers;

    private QuizAttempt() { }

    public QuizAttempt(Guid userId, Guid quizId)
    {
        UserId = userId;
        QuizId = quizId;
        StartedAt = DateTime.UtcNow;
    }
}
