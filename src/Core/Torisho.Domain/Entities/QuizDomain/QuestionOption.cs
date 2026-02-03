using Torisho.Domain.Common;

namespace Torisho.Domain.Entities.QuizDomain;

public sealed class QuestionOption : BaseEntity
{
    public Guid QuestionId { get; private set; }
    public Question Question { get; private set; } = default!;

    public string OptionText { get; private set; } = default!;
    public bool IsCorrect { get; private set; }

    private QuestionOption() { }

    public QuestionOption(Guid questionId, string optionText, bool isCorrect)
    {
        QuestionId = questionId;
        OptionText = optionText;
        IsCorrect = isCorrect;
    }
}
