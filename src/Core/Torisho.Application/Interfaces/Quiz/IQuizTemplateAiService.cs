namespace Torisho.Application.Interfaces.Quiz;

public interface IQuizTemplateAiService
{
    bool IsEnabled { get; }

    Task<string?> TryRewriteQuestionAsync(QuizAiRewriteRequest request, CancellationToken ct = default);
}

public record QuizAiRewriteRequest
{
    public string Skill { get; init; } = string.Empty;
    public string Purpose { get; init; } = string.Empty;
    public string QuizType { get; init; } = string.Empty;
    public string TemplateMode { get; init; } = string.Empty;
    public string Level { get; init; } = string.Empty;
    public string LearningContent { get; init; } = string.Empty;
    public string WeakSkills { get; init; } = string.Empty;
    public string Mistakes { get; init; } = string.Empty;
    public string CompletedTopics { get; init; } = string.Empty;
    public string CurrentQuestion { get; init; } = string.Empty;
    public string CorrectOption { get; init; } = string.Empty;
    public string RequiredGrammarPoint { get; init; } = string.Empty;
    public string RequiredKanji { get; init; } = string.Empty;
    public IReadOnlyList<string> AnchorTerms { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> Distractors { get; init; } = Array.Empty<string>();
}
