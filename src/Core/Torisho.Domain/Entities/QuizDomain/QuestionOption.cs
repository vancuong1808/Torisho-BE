using Torisho.Domain.Common;

namespace Torisho.Domain.Entities.QuizDomain;

public sealed class QuestionOption : BaseEntity
{
    public Guid QuestionId { get; private set; }
    public Question? Question { get; private set; }

    public string OptionText { get; private set; } = string.Empty;
    public bool IsCorrect { get; private set; }

    private QuestionOption() { }

    public QuestionOption(Guid questionId, string optionText, bool isCorrect)
    {
        if (questionId == Guid.Empty)
            throw new ArgumentException("QuestionId cannot be empty", nameof(questionId));
        if (string.IsNullOrWhiteSpace(optionText))
            throw new ArgumentException("OptionText is required", nameof(optionText));

        QuestionId = questionId;
        OptionText = optionText;
        IsCorrect = isCorrect;
    }
}
