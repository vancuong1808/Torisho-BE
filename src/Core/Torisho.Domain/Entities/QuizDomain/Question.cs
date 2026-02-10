using Torisho.Domain.Common;

namespace Torisho.Domain.Entities.QuizDomain;

public sealed class Question : QuizComponent, IAggregateRoot
{
    // DDD: Aggregate - Question manages Options through domain methods
    public IReadOnlyCollection<QuestionOption> Options => _options;
    private readonly HashSet<QuestionOption> _options = new();

    public Guid QuizId { get; private set; }
    public Quiz? Quiz { get; private set; }

    public string Content { get; private set; } = string.Empty;
    public int Order { get; private set; }


    private Question() { }

    public Question(Guid quizId, string content, int order)
    {
        if (quizId == Guid.Empty)
            throw new ArgumentException("QuizId cannot be empty", nameof(quizId));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required", nameof(content));
        if (order < 0)
            throw new ArgumentException("Order must be non-negative", nameof(order));

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
