using Torisho.Domain.Common;
using Torisho.Domain.Entities.QuizDomain;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Entities.ContentDomain;

public sealed class Grammar : LearningContent, IAggregateRoot
{
    public string Explanation { get; private set; } = string.Empty;
    public string Example { get; private set; } = string.Empty;
    public string? UsageJson { get; private set; }

    private Grammar() { }

    public Grammar(string title, Guid levelId, string explanation, string example, 
        string? usageJson = null) : base(title, levelId)
    {
        if (string.IsNullOrWhiteSpace(explanation))
            throw new ArgumentException("Explanation is required", nameof(explanation));
        if (string.IsNullOrWhiteSpace(example))
            throw new ArgumentException("Example is required", nameof(example));

        Explanation = explanation;
        Example = example;
        UsageJson = usageJson;
    }

    public override void Display()
    {
        throw new NotImplementedException();
    }

    public override Quiz CreateQuiz()
        => new(QuizType.Grammar, Id);
}
