using Torisho.Domain.Enums;

namespace Torisho.Application.DTOs.Quiz;

public sealed record QuizDetailDto
{
    public Guid QuizId { get; init; }
    public QuizType Type { get; init; }
    public Guid TargetContentId { get; init; }
    public string Title { get; init; } = string.Empty;
    public bool IsDaily { get; init; }
    public DateOnly? DailyDate { get; init; }
    public int EstimatedMinutes { get; init; }
    public string? Tip { get; init; }
    public int QuestionCount { get; init; }
    public IReadOnlyList<QuizQuestionDto> Questions { get; init; } = Array.Empty<QuizQuestionDto>();
}

public sealed record QuizQuestionDto
{
    public Guid QuestionId { get; init; }
    public int Order { get; init; }
    public string Skill { get; init; } = string.Empty;
    public string Source { get; init; } = string.Empty;
    public string Difficulty { get; init; } = "medium";
    public string? Topic { get; init; }
    public string Content { get; init; } = string.Empty;
    public IReadOnlyList<QuizOptionDto> Options { get; init; } = Array.Empty<QuizOptionDto>();
}

public sealed record QuizOptionDto
{
    public Guid OptionId { get; init; }
    public string Text { get; init; } = string.Empty;
}

public sealed record DailyQuizDto
{
    public DateOnly Date { get; init; }
    public bool IsCached { get; init; }
    public QuizDetailDto Quiz { get; init; } = new();
}

public sealed record SubmitQuizRequest
{
    public IReadOnlyList<QuizAnswerSubmissionDto> Answers { get; init; } = Array.Empty<QuizAnswerSubmissionDto>();
}

public sealed record QuizAnswerSubmissionDto
{
    public Guid QuestionId { get; init; }
    public Guid SelectedOptionId { get; init; }
}

public sealed record QuizSubmitResultDto
{
    public Guid AttemptId { get; init; }
    public Guid QuizId { get; init; }
    public float Score { get; init; }
    public int TotalQuestions { get; init; }
    public int CorrectAnswers { get; init; }
    public DateTime StartedAt { get; init; }
    public DateTime CompletedAt { get; init; }
    public IReadOnlyList<QuizSkillScoreDto> SkillScores { get; init; } = Array.Empty<QuizSkillScoreDto>();
    public IReadOnlyList<QuizQuestionResultDto> Questions { get; init; } = Array.Empty<QuizQuestionResultDto>();
}

public sealed record QuizQuestionResultDto
{
    public Guid QuestionId { get; init; }
    public string Skill { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public Guid SelectedOptionId { get; init; }
    public string SelectedOptionText { get; init; } = string.Empty;
    public Guid CorrectOptionId { get; init; }
    public string CorrectOptionText { get; init; } = string.Empty;
    public bool IsCorrect { get; init; }
}

public sealed record QuizSkillScoreDto
{
    public string Skill { get; init; } = string.Empty;
    public int Total { get; init; }
    public int Correct { get; init; }
    public float Score { get; init; }
}

public sealed record QuizPregenerateRequest
{
    public JLPTLevel? LevelCode { get; init; }
    public int? ChapterOrder { get; init; }
    public bool ForceRegenerate { get; init; }
    public IReadOnlyList<QuizType>? Types { get; init; }
    public bool? UseAiGeneration { get; init; }
    public string? TemplateMode { get; init; }
}

public sealed record LessonQuizPregenerateResultDto
{
    public int TotalLessons { get; init; }
    public int CreatedCount { get; init; }
    public int SkippedCount { get; init; }
    public int FailedCount { get; init; }
    public IReadOnlyList<LessonQuizPregenerateItemDto> Items { get; init; } = Array.Empty<LessonQuizPregenerateItemDto>();
}

public sealed record LessonQuizPregenerateItemDto
{
    public Guid LessonId { get; init; }
    public string LessonSlug { get; init; } = string.Empty;
    public string LessonTitle { get; init; } = string.Empty;
    public QuizType Type { get; init; }
    public string Status { get; init; } = string.Empty;
    public Guid? QuizId { get; init; }
    public string? Message { get; init; }
}