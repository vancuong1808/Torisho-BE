using Torisho.Domain.Common;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Entities.QuizDomain;

public sealed class Quiz : QuizComponent, IAggregateRoot
{
    private readonly HashSet<Question> _questions = new();

    public QuizType Type { get; private set; }
    public Guid TargetContentId { get; private set; }

    public IReadOnlyCollection<Question> Questions => _questions;

    private Quiz() { }

    public Quiz(QuizType type, Guid targetContentId)
    {
        Type = type;
        TargetContentId = targetContentId;
    }

    public override bool Validate() => true;
}
