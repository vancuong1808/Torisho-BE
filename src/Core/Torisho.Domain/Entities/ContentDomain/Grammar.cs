using Torisho.Domain.Common;
using Torisho.Domain.Entities.QuizDomain;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Entities.ContentDomain;

public sealed class Grammar : LearningContent
{
    public string Explanation { get; set; } = string.Empty;
    public string Example { get; set; } = string.Empty;
    public string? UsageJson { get; set; }

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
