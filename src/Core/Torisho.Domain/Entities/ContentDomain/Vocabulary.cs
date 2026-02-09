using Torisho.Domain.Common;
using Torisho.Domain.Entities.DictionaryDomain;
using Torisho.Domain.Entities.QuizDomain;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Entities.ContentDomain;

public sealed class Vocabulary : LearningContent
{
    public Guid DictionaryEntryId { get; private set; }
    public DictionaryEntry DictionaryEntry { get; private set; } = null!;

    private Vocabulary() { }

    public Vocabulary(string title, Guid levelId, Guid dictionaryEntryId)
        : base(title, levelId)
    {
        DictionaryEntryId = dictionaryEntryId;
    }

    public override void Display() { }

    public override Quiz CreateQuiz()
        => new(QuizType.Vocabulary, Id);
}
