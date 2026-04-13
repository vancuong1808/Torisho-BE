using Torisho.Domain.Enums;

namespace Torisho.Application.DTOs.Learning;

public sealed record ChapterLessonsResponseDto
{
    public Guid ChapterId { get; init; }
    public string ChapterTitle { get; init; } = string.Empty;
    public int ChapterOrder { get; init; }

    public Guid LevelId { get; init; }
    public JLPTLevel LevelCode { get; init; }
    public string LevelName { get; init; } = string.Empty;

    public int TotalLessons { get; init; }
    public List<LessonListItemDto> Lessons { get; init; } = new();
}

public sealed record LessonListItemDto
{
    public Guid Id { get; init; }
    public string Slug { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Order { get; init; }
    public JLPTLevel SourceLevel { get; init; }
    public bool HasQuiz { get; init; }

    public int VocabularyCount { get; init; }
    public int GrammarCount { get; init; }
    public int ReadingCount { get; init; }
}
