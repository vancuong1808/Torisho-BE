using Torisho.Domain.Common;
using Torisho.Domain.Entities.QuizDomain;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Entities.ContentDomain;

public sealed class Grammar : LearningContent, IAggregateRoot
{
    public string Explanation { get; private set; } = default!;
    public string Example { get; private set; } = default!;
    public string UsageJson { get; private set; } = default!;

    private Grammar() { }

    public Grammar(string title, Guid levelId, string explanation, string example, string usageJson)
        : base(title, levelId)
    {
        Explanation = explanation;
        Example = example;
        UsageJson = usageJson;
    }

    public override void Display() { }

    public override Quiz CreateQuiz()
        => new(QuizType.Grammar, Id);
}
