using Torisho.Domain.Common;

namespace Torisho.Domain.Entities.QuizDomain;

public sealed class QuizAnswer : BaseEntity
{
    public Guid AttemptId { get; private set; }
    public QuizAttempt? Attempt { get; private set; }

    public Guid QuestionId { get; private set; }
    public Question? Question { get; private set; }

    public Guid SelectedOptionId { get; private set; }
    public QuestionOption? SelectedOption { get; private set; }

    public bool IsCorrect { get; private set; }

    private QuizAnswer() { }

    public QuizAnswer(Guid attemptId, Guid questionId, Guid selectedOptionId, bool isCorrect)
    {
        if (attemptId == Guid.Empty)
            throw new ArgumentException("AttemptId cannot be empty", nameof(attemptId));
        if (questionId == Guid.Empty)
            throw new ArgumentException("QuestionId cannot be empty", nameof(questionId));
        if (selectedOptionId == Guid.Empty)
            throw new ArgumentException("SelectedOptionId cannot be empty", nameof(selectedOptionId));

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
