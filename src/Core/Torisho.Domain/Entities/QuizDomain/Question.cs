using Torisho.Domain.Common;

namespace Torisho.Domain.Entities.QuizDomain;

public sealed class Question : QuizComponent, IAggregateRoot
{
    // DDD: Aggregate - Question manages Options through domain methods
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

    public void AddOption(QuestionOption option)
    {
        ArgumentNullException.ThrowIfNull(option);
        _options.Add(option);
    }

    public void RemoveOption(QuestionOption option)
    {
        ArgumentNullException.ThrowIfNull(option);
        _options.Remove(option);
    }

    public QuestionOption? GetCorrectOption() => _options.FirstOrDefault(o => o.IsCorrect);
}
