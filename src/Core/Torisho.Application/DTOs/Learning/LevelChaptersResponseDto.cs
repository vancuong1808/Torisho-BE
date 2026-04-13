using Torisho.Domain.Enums;

namespace Torisho.Application.DTOs.Learning;

public sealed record LevelChaptersResponseDto
{
    public Guid LevelId { get; init; }
    public JLPTLevel LevelCode { get; init; }
    public string LevelName { get; init; } = string.Empty;
    public int TotalChapters { get; init; }
    public List<ChapterListItemDto> Chapters { get; init; } = new();
}

public sealed record ChapterListItemDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int Order { get; init; }
    public int LessonCount { get; init; }
}
