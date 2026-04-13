using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Application.DTOs.Learning;
using Torisho.Application.Interfaces.Learning;
using Torisho.Domain.Entities.LearningDomain;
using Torisho.Domain.Enums;

namespace Torisho.Application.Services.Learning;

public sealed class LearningQueryService : ILearningQueryService
{
    private readonly IDataContext _context;

    public LearningQueryService(IDataContext context)
    {
        _context = context;
    }

    public async Task<LevelChaptersResponseDto?> GetLevelChaptersAsync(JLPTLevel levelCode, CancellationToken ct = default)
    {
        var level = await _context.Set<Level>()
            .AsNoTracking()
            .Where(x => x.Code == levelCode)
            .Select(x => new
            {
                x.Id,
                x.Code,
                x.Name
            })
            .FirstOrDefaultAsync(ct);

        if (level is null)
            return null;

        var chapters = await _context.Set<Chapter>()
            .AsNoTracking()
            .Where(x => x.LevelId == level.Id)
            .OrderBy(x => x.Order)
            .Select(x => new ChapterListItemDto
            {
                Id = x.Id,
                Title = x.Title,
                Order = x.Order,
                LessonCount = x.Lessons.Count
            })
            .ToListAsync(ct);

        return new LevelChaptersResponseDto
        {
            LevelId = level.Id,
            LevelCode = level.Code,
            LevelName = level.Name,
            TotalChapters = chapters.Count,
            Chapters = chapters
        };
    }

    public async Task<ChapterLessonsResponseDto?> GetChapterLessonsByIdAsync(Guid chapterId, CancellationToken ct = default)
    {
        if (chapterId == Guid.Empty)
            throw new ArgumentException("ChapterId cannot be empty", nameof(chapterId));

        var chapterHeader = await _context.Set<Chapter>()
            .AsNoTracking()
            .Join(
                _context.Set<Level>().AsNoTracking(),
                chapter => chapter.LevelId,
                level => level.Id,
                (chapter, level) => new { chapter, level })
            .Where(x => x.chapter.Id == chapterId)
            .Select(x => new ChapterHeaderDto
            {
                ChapterId = x.chapter.Id,
                ChapterTitle = x.chapter.Title,
                ChapterOrder = x.chapter.Order,
                LevelId = x.level.Id,
                LevelCode = x.level.Code,
                LevelName = x.level.Name
            })
            .FirstOrDefaultAsync(ct);

        if (chapterHeader is null)
            return null;

        var lessons = await GetLessonListByChapterIdAsync(chapterId, ct);

        return new ChapterLessonsResponseDto
        {
            ChapterId = chapterHeader.ChapterId,
            ChapterTitle = chapterHeader.ChapterTitle,
            ChapterOrder = chapterHeader.ChapterOrder,
            LevelId = chapterHeader.LevelId,
            LevelCode = chapterHeader.LevelCode,
            LevelName = chapterHeader.LevelName,
            TotalLessons = lessons.Count,
            Lessons = lessons
        };
    }

    public async Task<ChapterLessonsResponseDto?> GetChapterLessonsByLevelAsync(
        JLPTLevel levelCode,
        int chapterOrder,
        CancellationToken ct = default)
    {
        if (chapterOrder <= 0)
            throw new ArgumentException("ChapterOrder must be greater than 0", nameof(chapterOrder));

        var chapterHeader = await _context.Set<Chapter>()
            .AsNoTracking()
            .Join(
                _context.Set<Level>().AsNoTracking(),
                chapter => chapter.LevelId,
                level => level.Id,
                (chapter, level) => new { chapter, level })
            .Where(x => x.level.Code == levelCode && x.chapter.Order == chapterOrder)
            .Select(x => new ChapterHeaderDto
            {
                ChapterId = x.chapter.Id,
                ChapterTitle = x.chapter.Title,
                ChapterOrder = x.chapter.Order,
                LevelId = x.level.Id,
                LevelCode = x.level.Code,
                LevelName = x.level.Name
            })
            .FirstOrDefaultAsync(ct);

        if (chapterHeader is null)
            return null;

        var lessons = await GetLessonListByChapterIdAsync(chapterHeader.ChapterId, ct);

        return new ChapterLessonsResponseDto
        {
            ChapterId = chapterHeader.ChapterId,
            ChapterTitle = chapterHeader.ChapterTitle,
            ChapterOrder = chapterHeader.ChapterOrder,
            LevelId = chapterHeader.LevelId,
            LevelCode = chapterHeader.LevelCode,
            LevelName = chapterHeader.LevelName,
            TotalLessons = lessons.Count,
            Lessons = lessons
        };
    }

    public async Task<LessonDetailDto?> GetLessonBySlugAsync(string slug, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Slug is required", nameof(slug));

        var normalizedSlug = slug.Trim();

        var lesson = await _context.Set<Lesson>()
            .AsNoTracking()
            .Include(l => l.Chapter!)
                .ThenInclude(c => c.Level)
            .Include(l => l.VocabularyItems.OrderBy(item => item.SortOrder))
            .Include(l => l.GrammarItems.OrderBy(item => item.SortOrder))
            .Include(l => l.ReadingItems.OrderBy(item => item.SortOrder))
            .FirstOrDefaultAsync(l => l.Slug == normalizedSlug, ct);

        if (lesson is null || lesson.Chapter is null || lesson.Chapter.Level is null)
            return null;

        return new LessonDetailDto
        {
            Id = lesson.Id,
            Slug = lesson.Slug,
            Title = lesson.Title,
            Description = lesson.Description,
            Order = lesson.Order,
            SourceLevel = lesson.SourceLevel,
            HasQuiz = lesson.HasQuiz(),

            ChapterId = lesson.ChapterId,
            ChapterTitle = lesson.Chapter.Title,
            ChapterOrder = lesson.Chapter.Order,

            LevelId = lesson.Chapter.LevelId,
            LevelCode = lesson.Chapter.Level.Code,
            LevelName = lesson.Chapter.Level.Name,

            Vocabulary = lesson.VocabularyItems
                .OrderBy(x => x.SortOrder)
                .Select(x => new LessonVocabularyItemDto
                {
                    Id = x.Id,
                    SortOrder = x.SortOrder,
                    Term = x.Term,
                    Reading = x.Reading,
                    Note = x.Note,
                    IsCommon = x.IsCommon,
                    Meanings = ParseJsonOrDefault(x.MeaningsJson),
                    Examples = ParseJsonOrNull(x.ExamplesJson),
                    OtherForms = ParseJsonOrNull(x.OtherFormsJson),
                    JlptTags = ParseJsonOrNull(x.JlptTagsJson)
                })
                .ToList(),

            Grammar = lesson.GrammarItems
                .OrderBy(x => x.SortOrder)
                .Select(x => new LessonGrammarItemDto
                {
                    Id = x.Id,
                    SortOrder = x.SortOrder,
                    GrammarPoint = x.GrammarPoint,
                    MeaningEn = x.MeaningEn,
                    DetailUrl = x.DetailUrl,
                    LevelHint = x.LevelHint,
                    Explanation = x.Explanation,
                    Usage = ParseJsonOrNull(x.UsageJson),
                    Examples = ParseJsonOrNull(x.ExamplesJson)
                })
                .ToList(),

            Reading = lesson.ReadingItems
                .OrderBy(x => x.SortOrder)
                .Select(x => new LessonReadingItemDto
                {
                    Id = x.Id,
                    SortOrder = x.SortOrder,
                    Title = x.Title,
                    Content = x.Content,
                    Translation = x.Translation,
                    Url = x.Url,
                    LevelHint = x.LevelHint,
                    Source = x.Source
                })
                .ToList()
        };
    }

    private async Task<List<LessonListItemDto>> GetLessonListByChapterIdAsync(Guid chapterId, CancellationToken ct)
    {
        return await _context.Set<Lesson>()
            .AsNoTracking()
            .Where(l => l.ChapterId == chapterId)
            .OrderBy(l => l.Order)
            .Select(l => new LessonListItemDto
            {
                Id = l.Id,
                Slug = l.Slug,
                Title = l.Title,
                Description = l.Description,
                Order = l.Order,
                SourceLevel = l.SourceLevel,
                HasQuiz = l.QuizId.HasValue,
                VocabularyCount = l.VocabularyItems.Count,
                GrammarCount = l.GrammarItems.Count,
                ReadingCount = l.ReadingItems.Count
            })
            .ToListAsync(ct);
    }

    private static JsonElement ParseJsonOrDefault(string? json, string defaultJson = "[]")
    {
        var payload = string.IsNullOrWhiteSpace(json) ? defaultJson : json;

        using var document = JsonDocument.Parse(payload);
        return document.RootElement.Clone();
    }

    private static JsonElement? ParseJsonOrNull(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }

    private sealed record ChapterHeaderDto
    {
        public Guid ChapterId { get; init; }
        public string ChapterTitle { get; init; } = string.Empty;
        public int ChapterOrder { get; init; }
        public Guid LevelId { get; init; }
        public JLPTLevel LevelCode { get; init; }
        public string LevelName { get; init; } = string.Empty;
    }
}
