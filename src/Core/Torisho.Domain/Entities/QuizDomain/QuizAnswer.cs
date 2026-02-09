using Torisho.Domain.Common;

namespace Torisho.Domain.Entities.QuizDomain;

public sealed class QuizAnswer : BaseEntity
{
    public Guid AttemptId { get; private set; }
    public QuizAttempt Attempt { get; private set; } = default!;

    public Guid QuestionId { get; private set; }
    public Question Question { get; private set; } = default!;

    public Guid SelectedOptionId { get; private set; }
    public QuestionOption SelectedOption { get; private set; } = default!;

    public bool IsCorrect { get; private set; }

    private QuizAnswer() { }

    public QuizAnswer(Guid attemptId, Guid questionId, Guid selectedOptionId, bool isCorrect)
    {
        AttemptId = attemptId;
        QuestionId = questionId;
        SelectedOptionId = selectedOptionId;
        IsCorrect = isCorrect;
    }

    public bool Validate()
    {
        // Basic validate: ensure ids are present
        return AttemptId != Guid.Empty && QuestionId != Guid.Empty && SelectedOptionId != Guid.Empty;
    }
}
