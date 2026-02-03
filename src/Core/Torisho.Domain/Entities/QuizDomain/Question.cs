using Torisho.Domain.Common;

namespace Torisho.Domain.Entities.QuizDomain;

public sealed class Question : QuizComponent
{
    private readonly HashSet<QuestionOption> _options = new();

    public Guid QuizId { get; private set; }
    public Quiz Quiz { get; private set; } = default!;

    public string Content { get; private set; } = default!;
    public int Order { get; private set; }

    public IReadOnlyCollection<QuestionOption> Options => _options;

    private Question() { }

    public Question(Guid quizId, string content, int order)
    {
        QuizId = quizId;
        Content = content;
        Order = order;
    }

    public override bool Validate() => true;
}
