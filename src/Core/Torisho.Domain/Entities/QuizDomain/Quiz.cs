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

    public void AddQuestion(Question question)
    {
        if (!_questions.Contains(question)) _questions.Add(question);
    }

    public void RemoveQuestion(Question question)
    {
        if (_questions.Contains(question)) _questions.Remove(question);
    }

    public float Evaluate(IDictionary<Guid, Guid> answers)
    {
        // answers: questionId -> selectedOptionId
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<Question> GetQuestions() => Questions;
}
