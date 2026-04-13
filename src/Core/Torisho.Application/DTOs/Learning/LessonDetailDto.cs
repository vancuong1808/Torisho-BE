using System.Text.Json;
using Torisho.Domain.Enums;

namespace Torisho.Application.DTOs.Learning;

public sealed record LessonDetailDto
{
    public Guid Id { get; init; }
    public string Slug { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Order { get; init; }
    public JLPTLevel SourceLevel { get; init; }
    public bool HasQuiz { get; init; }

    public Guid ChapterId { get; init; }
    public string ChapterTitle { get; init; } = string.Empty;
    public int ChapterOrder { get; init; }

    public Guid LevelId { get; init; }
    public JLPTLevel LevelCode { get; init; }
    public string LevelName { get; init; } = string.Empty;

    public List<LessonVocabularyItemDto> Vocabulary { get; init; } = new();
    public List<LessonGrammarItemDto> Grammar { get; init; } = new();
    public List<LessonReadingItemDto> Reading { get; init; } = new();
}

public sealed record LessonVocabularyItemDto
{
    public Guid Id { get; init; }
    public int SortOrder { get; init; }
    public string Term { get; init; } = string.Empty;
    public string Reading { get; init; } = string.Empty;
    public string? Note { get; init; }
    public bool IsCommon { get; init; }

    public JsonElement Meanings { get; init; }
    public JsonElement? Examples { get; init; }
    public JsonElement? OtherForms { get; init; }
    public JsonElement? JlptTags { get; init; }
}

public sealed record LessonGrammarItemDto
{
    public Guid Id { get; init; }
    public int SortOrder { get; init; }
    public string GrammarPoint { get; init; } = string.Empty;
    public string MeaningEn { get; init; } = string.Empty;
    public string? DetailUrl { get; init; }
    public string? LevelHint { get; init; }
    public string? Explanation { get; init; }
    public JsonElement? Usage { get; init; }
    public JsonElement? Examples { get; init; }
}

public sealed record LessonReadingItemDto
{
    public Guid Id { get; init; }
    public int SortOrder { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string? Translation { get; init; }
    public string? Url { get; init; }
    public string? LevelHint { get; init; }
    public string? Source { get; init; }
}
