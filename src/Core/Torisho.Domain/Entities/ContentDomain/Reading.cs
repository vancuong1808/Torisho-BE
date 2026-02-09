using Torisho.Domain.Common;
using Torisho.Domain.Entities.QuizDomain;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Entities.ContentDomain;

public sealed class Reading : LearningContent
{
    public string Content { get; set; } = string.Empty;
    public string? Translation { get; set; }

    private Reading() { }

    public Reading(string title, Guid levelId, string content, string translation)
        : base(title, levelId)
    {
        Content = content;
        Translation = translation;
    }

    public override void Display() { }

    public override Quiz CreateQuiz()
        => new(QuizType.Reading, Id);
}
