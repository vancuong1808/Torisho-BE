namespace Torisho.Application.DTOs.Learning;

public sealed record LessonHeartbeatRequest
{
    public float LessonProgressPercent { get; init; }
    public float? ChapterProgressPercent { get; init; }
    public float? LevelProgressPercent { get; init; }

    public float? VocabularyProgress { get; init; }
    public float? GrammarProgress { get; init; }
    public float? ReadingProgress { get; init; }

    public string? Section { get; init; }
}
