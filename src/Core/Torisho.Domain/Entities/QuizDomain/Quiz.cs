using Torisho.Domain.Common;
using Torisho.Domain.Enums;
namespace Torisho.Domain.Entities.QuizDomain;

public sealed class Quiz : QuizComponent, IAggregateRoot
{
    public QuizType Type { get; private set; }
    public Guid TargetContentId { get; private set; }
    
    // DDD: Aggregate - Quiz manages Questions through domain methods
    private readonly HashSet<Question> _questions = new();
    public IReadOnlyCollection<Question> Questions => _questions;

    private Quiz() { }

    public Quiz(QuizType type, Guid targetContentId)
    {
        if (targetContentId == Guid.Empty)
            throw new ArgumentException("TargetContentId cannot be empty", nameof(targetContentId));

        Type = type;
        TargetContentId = targetContentId;
    }

    public override bool Validate() => true;

    public void AddQuestion(Question question)
    {
        ArgumentNullException.ThrowIfNull(question);
        _questions.Add(question);
    }

    public void RemoveQuestion(Question question)
    {
        ArgumentNullException.ThrowIfNull(question);
        _questions.Remove(question);
    }

    public float Evaluate(IDictionary<Guid, Guid> answers)
    {
        // answers: questionId -> selectedOptionId
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<Question> GetQuestions() => Questions;
}
