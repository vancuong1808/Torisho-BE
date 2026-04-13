using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Application.Interfaces.Learning;
using Torisho.Domain.Entities.LearningDomain;
using Torisho.Domain.Entities.ProgressDomain;
using Torisho.Domain.Enums;

namespace Torisho.Infrastructure.Services.Learning;

public sealed class CurriculumImportService : ICurriculumImportService
{
    private readonly IDataContext _context;

    public CurriculumImportService(IDataContext context)
    {
        _context = context;
    }

    public async Task<CurriculumImportResult> ImportFromFolderAsync(string folderPath, bool clearExisting = false, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
            throw new ArgumentException("Folder path is required", nameof(folderPath));
        if (!Directory.Exists(folderPath))
            throw new DirectoryNotFoundException($"Folder not found: {folderPath}");

        var files = Directory.GetFiles(folderPath, "*.json", SearchOption.AllDirectories)
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var errors = new List<string>();
        if (files.Count == 0)
        {
            return new CurriculumImportResult(
                FilesDiscovered: 0,
                FilesProcessed: 0,
                FilesSkipped: 0,
                LevelsCreated: 0,
                ChaptersCreated: 0,
                LessonsCreated: 0,
                LessonsUpdated: 0,
                VocabularyItemsInserted: 0,
                GrammarItemsInserted: 0,
                ReadingItemsInserted: 0,
                ClearedExisting: clearExisting,
                Errors: errors);
        }

        var levelsCreated = 0;
        var chaptersCreated = 0;
        var lessonsCreated = 0;
        var lessonsUpdated = 0;
        var filesProcessed = 0;
        var filesSkipped = 0;
        var vocabInserted = 0;
        var grammarInserted = 0;
        var readingInserted = 0;

        if (clearExisting)
        {
            // Clear progress first because LearningProgress -> Level uses Restrict delete behavior.
            await _context.Set<ChapterProgress>().ExecuteDeleteAsync(ct);
            await _context.Set<LearningProgress>().ExecuteDeleteAsync(ct);
            await _context.Set<DailyActivities>().ExecuteDeleteAsync(ct);

            await _context.Set<LessonVocabularyItem>().ExecuteDeleteAsync(ct);
            await _context.Set<LessonGrammarItem>().ExecuteDeleteAsync(ct);
            await _context.Set<LessonReadingItem>().ExecuteDeleteAsync(ct);
            await _context.Set<Lesson>().ExecuteDeleteAsync(ct);
            await _context.Set<Chapter>().ExecuteDeleteAsync(ct);
            await _context.Set<Level>().ExecuteDeleteAsync(ct);
        }

        var levels = await _context.Set<Level>().ToListAsync(ct);
        var chapters = await _context.Set<Chapter>().ToListAsync(ct);
        var lessons = await _context.Set<Lesson>().ToListAsync(ct);

        var levelByCode = levels.ToDictionary(x => x.Code, x => x);
        var chapterByKey = chapters.ToDictionary(x => (x.LevelId, x.Order), x => x);
        var lessonBySlug = lessons
            .Where(x => !string.IsNullOrWhiteSpace(x.Slug))
            .ToDictionary(x => x.Slug, x => x, StringComparer.OrdinalIgnoreCase);

        foreach (var file in files)
        {
            if (ct.IsCancellationRequested)
                break;

            ParsedLessonFile? parsed;
            try
            {
                parsed = await ParseFileAsync(file, ct);
            }
            catch (Exception ex)
            {
                filesSkipped++;
                errors.Add($"{Path.GetFileName(file)}: {ex.Message}");
                continue;
            }

            if (parsed is null)
            {
                filesSkipped++;
                errors.Add($"{Path.GetFileName(file)}: invalid JSON structure");
                continue;
            }

            if (!levelByCode.TryGetValue(parsed.LevelCode, out var level))
            {
                level = new Level(
                    code: parsed.LevelCode,
                    name: string.IsNullOrWhiteSpace(parsed.LevelName) ? $"JLPT {parsed.LevelCode}" : parsed.LevelName,
                    description: null,
                    order: GetLevelOrder(parsed.LevelCode));

                await _context.Set<Level>().AddAsync(level, ct);
                levelByCode[parsed.LevelCode] = level;
                levelsCreated++;
            }

            if (!chapterByKey.TryGetValue((level.Id, parsed.ChapterOrder), out var chapter))
            {
                chapter = new Chapter(
                    levelId: level.Id,
                    title: string.IsNullOrWhiteSpace(parsed.ChapterTitle) ? $"Chapter {parsed.ChapterOrder}" : parsed.ChapterTitle,
                    description: null,
                    order: parsed.ChapterOrder);

                await _context.Set<Chapter>().AddAsync(chapter, ct);
                chapterByKey[(level.Id, parsed.ChapterOrder)] = chapter;
                chaptersCreated++;
            }

            if (!lessonBySlug.TryGetValue(parsed.LessonSlug, out var lesson))
            {
                lesson = new Lesson(
                    chapterId: chapter.Id,
                    slug: parsed.LessonSlug,
                    title: parsed.LessonTitle,
                    description: null,
                    order: parsed.LessonOrder,
                    sourceLevel: parsed.LevelCode);

                await _context.Set<Lesson>().AddAsync(lesson, ct);
                lessonBySlug[parsed.LessonSlug] = lesson;
                lessonsCreated++;
            }
            else
            {
                lesson.UpdateStructure(
                    slug: parsed.LessonSlug,
                    title: parsed.LessonTitle,
                    description: null,
                    order: parsed.LessonOrder,
                    sourceLevel: parsed.LevelCode);

                // Keep structure consistent if chapter changes over time.
                if (lesson.ChapterId != chapter.Id)
                {
                    var db = _context as DbContext;
                    if (db is not null)
                        db.Entry(lesson).Property(x => x.ChapterId).CurrentValue = chapter.Id;
                }

                lessonsUpdated++;
            }

            await _context.Set<LessonVocabularyItem>()
                .Where(x => x.LessonId == lesson.Id)
                .ExecuteDeleteAsync(ct);

            await _context.Set<LessonGrammarItem>()
                .Where(x => x.LessonId == lesson.Id)
                .ExecuteDeleteAsync(ct);

            await _context.Set<LessonReadingItem>()
                .Where(x => x.LessonId == lesson.Id)
                .ExecuteDeleteAsync(ct);

            for (var i = 0; i < parsed.Vocabularies.Count; i++)
            {
                var item = parsed.Vocabularies[i];
                await _context.Set<LessonVocabularyItem>().AddAsync(new LessonVocabularyItem(
                    lessonId: lesson.Id,
                    sortOrder: i + 1,
                    term: item.Term,
                    reading: item.Reading,
                    meaningsJson: item.MeaningsJson,
                    note: item.Note,
                    examplesJson: item.ExamplesJson,
                    otherFormsJson: item.OtherFormsJson,
                    isCommon: item.IsCommon,
                    jlptTagsJson: item.JlptTagsJson), ct);
                vocabInserted++;
            }

            for (var i = 0; i < parsed.Grammars.Count; i++)
            {
                var item = parsed.Grammars[i];
                if (string.IsNullOrWhiteSpace(item.GrammarPoint))
                {
                    errors.Add($"{Path.GetFileName(file)}: grammar[{i}] missing grammar_point");
                    continue;
                }

                await _context.Set<LessonGrammarItem>().AddAsync(new LessonGrammarItem(
                    lessonId: lesson.Id,
                    sortOrder: i + 1,
                    grammarPoint: item.GrammarPoint,
                    meaningEn: item.MeaningEn ?? string.Empty,
                    detailUrl: item.DetailUrl,
                    levelHint: item.LevelHint,
                    explanation: item.Explanation,
                    usageJson: item.UsageJson,
                    examplesJson: item.ExamplesJson), ct);
                grammarInserted++;
            }

            for (var i = 0; i < parsed.Readings.Count; i++)
            {
                var item = parsed.Readings[i];
                if (string.IsNullOrWhiteSpace(item.Title) || string.IsNullOrWhiteSpace(item.Content))
                {
                    errors.Add($"{Path.GetFileName(file)}: reading[{i}] missing title/content");
                    continue;
                }

                await _context.Set<LessonReadingItem>().AddAsync(new LessonReadingItem(
                    lessonId: lesson.Id,
                    sortOrder: i + 1,
                    title: item.Title,
                    content: item.Content,
                    translation: item.Translation,
                    url: item.Url,
                    levelHint: item.LevelHint,
                    source: item.Source), ct);
                readingInserted++;
            }

            filesProcessed++;
        }

        await _context.SaveChangesAsync(ct);

        return new CurriculumImportResult(
            FilesDiscovered: files.Count,
            FilesProcessed: filesProcessed,
            FilesSkipped: filesSkipped,
            LevelsCreated: levelsCreated,
            ChaptersCreated: chaptersCreated,
            LessonsCreated: lessonsCreated,
            LessonsUpdated: lessonsUpdated,
            VocabularyItemsInserted: vocabInserted,
            GrammarItemsInserted: grammarInserted,
            ReadingItemsInserted: readingInserted,
            ClearedExisting: clearExisting,
            Errors: errors);
    }

    private static int GetLevelOrder(JLPTLevel level)
        => level switch
        {
            JLPTLevel.N5 => 1,
            JLPTLevel.N4 => 2,
            JLPTLevel.N3 => 3,
            JLPTLevel.N2 => 4,
            JLPTLevel.N1 => 5,
            _ => 99
        };

    private static async Task<ParsedLessonFile?> ParseFileAsync(string filePath, CancellationToken ct)
    {
        await using var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 1024 * 64,
            options: FileOptions.SequentialScan);

        using var document = await JsonDocument.ParseAsync(
            stream,
            new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            },
            ct);

        var root = document.RootElement;

        if (!root.TryGetProperty("level", out var levelEl) || levelEl.ValueKind != JsonValueKind.Object)
            return null;

        if (!TryGetString(levelEl, "code", out var levelCodeRaw) || string.IsNullOrWhiteSpace(levelCodeRaw))
            return null;

        if (!Enum.TryParse<JLPTLevel>(levelCodeRaw, ignoreCase: true, out var levelCode))
            return null;

        _ = TryGetString(levelEl, "name", out var levelName);

        if (!root.TryGetProperty("chapter", out var chapterEl) || chapterEl.ValueKind != JsonValueKind.Object)
            return null;

        _ = TryGetString(chapterEl, "title", out var chapterTitle);
        if (!TryGetInt(chapterEl, "sort_order", out var chapterOrder))
            return null;

        if (!root.TryGetProperty("lesson", out var lessonEl) || lessonEl.ValueKind != JsonValueKind.Object)
            return null;

        if (!TryGetString(lessonEl, "slug", out var lessonSlug) || string.IsNullOrWhiteSpace(lessonSlug))
            return null;
        if (!TryGetString(lessonEl, "title", out var lessonTitle) || string.IsNullOrWhiteSpace(lessonTitle))
            return null;
        if (!TryGetInt(lessonEl, "sort_order", out var lessonOrder))
            return null;

        var vocabularies = new List<ParsedVocabularyItem>();
        var grammars = new List<ParsedGrammarItem>();
        var readings = new List<ParsedReadingItem>();

        if (root.TryGetProperty("content", out var contentEl) && contentEl.ValueKind == JsonValueKind.Object)
        {
            if (contentEl.TryGetProperty("vocabulary", out var vocabEl) && vocabEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in vocabEl.EnumerateArray())
                {
                    if (item.ValueKind != JsonValueKind.Object)
                        continue;

                    var term = GetStringOrDefault(item, "term", string.Empty);
                    var reading = GetStringOrDefault(item, "reading", string.Empty);
                    if (string.IsNullOrWhiteSpace(term) || string.IsNullOrWhiteSpace(reading))
                        continue;

                    var note = GetNullableString(item, "note");
                    var meaningsJson = GetJsonRawOrDefault(item, "meanings", "[]");
                    var examplesJson = GetJsonRawOrNull(item, "examples");
                    var otherFormsJson = GetJsonRawOrNull(item, "other_forms");
                    var isCommon = TryGetBool(item, "is_common", out var common) && common;
                    var jlptTagsJson = GetJsonRawOrNull(item, "jlpt");

                    vocabularies.Add(new ParsedVocabularyItem(
                        Term: term,
                        Reading: reading,
                        Note: note,
                        MeaningsJson: meaningsJson,
                        ExamplesJson: examplesJson,
                        OtherFormsJson: otherFormsJson,
                        IsCommon: isCommon,
                        JlptTagsJson: jlptTagsJson));
                }
            }

            if (contentEl.TryGetProperty("grammar", out var grammarEl) && grammarEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in grammarEl.EnumerateArray())
                {
                    if (item.ValueKind != JsonValueKind.Object)
                        continue;

                    grammars.Add(new ParsedGrammarItem(
                        GrammarPoint: GetStringOrDefault(item, "grammar_point", string.Empty),
                        MeaningEn: GetNullableString(item, "meaning_en"),
                        DetailUrl: GetNullableString(item, "detail_url"),
                        LevelHint: GetNullableString(item, "level"),
                        Explanation: GetNullableString(item, "explanation"),
                        UsageJson: GetJsonRawOrNull(item, "usage"),
                        ExamplesJson: GetJsonRawOrNull(item, "examples")));
                }
            }

            if (contentEl.TryGetProperty("reading", out var readingEl) && readingEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in readingEl.EnumerateArray())
                {
                    if (item.ValueKind != JsonValueKind.Object)
                        continue;

                    readings.Add(new ParsedReadingItem(
                        Title: GetStringOrDefault(item, "title", string.Empty),
                        Content: GetStringOrDefault(item, "content", string.Empty),
                        Translation: GetNullableString(item, "translation"),
                        Url: GetNullableString(item, "url"),
                        LevelHint: GetNullableString(item, "level_hint"),
                        Source: GetNullableString(item, "source")));
                }
            }
        }

        return new ParsedLessonFile(
            LevelCode: levelCode,
            LevelName: levelName,
            ChapterTitle: chapterTitle,
            ChapterOrder: chapterOrder,
            LessonSlug: lessonSlug.Trim(),
            LessonTitle: lessonTitle.Trim(),
            LessonOrder: lessonOrder,
            Vocabularies: vocabularies,
            Grammars: grammars,
            Readings: readings);
    }

    private static bool TryGetString(JsonElement element, string propertyName, out string? value)
    {
        value = null;
        if (!element.TryGetProperty(propertyName, out var property))
            return false;

        if (property.ValueKind == JsonValueKind.String)
        {
            value = property.GetString();
            return true;
        }

        if (property.ValueKind == JsonValueKind.Null)
            return true;

        value = property.ToString();
        return true;
    }

    private static bool TryGetInt(JsonElement element, string propertyName, out int value)
    {
        value = default;
        if (!element.TryGetProperty(propertyName, out var property))
            return false;

        if (property.ValueKind == JsonValueKind.Number && property.TryGetInt32(out value))
            return true;

        if (property.ValueKind == JsonValueKind.String && int.TryParse(property.GetString(), out value))
            return true;

        return false;
    }

    private static bool TryGetBool(JsonElement element, string propertyName, out bool value)
    {
        value = default;
        if (!element.TryGetProperty(propertyName, out var property))
            return false;

        if (property.ValueKind == JsonValueKind.True)
        {
            value = true;
            return true;
        }

        if (property.ValueKind == JsonValueKind.False)
        {
            value = false;
            return true;
        }

        if (property.ValueKind == JsonValueKind.String && bool.TryParse(property.GetString(), out value))
            return true;

        return false;
    }

    private static string GetStringOrDefault(JsonElement element, string propertyName, string defaultValue)
        => TryGetString(element, propertyName, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : defaultValue;

    private static string? GetNullableString(JsonElement element, string propertyName)
        => TryGetString(element, propertyName, out var value) ? value : null;

    private static string? GetJsonRawOrNull(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
            return null;
        if (property.ValueKind == JsonValueKind.Null || property.ValueKind == JsonValueKind.Undefined)
            return null;
        return property.GetRawText();
    }

    private static string GetJsonRawOrDefault(JsonElement element, string propertyName, string defaultValue)
        => GetJsonRawOrNull(element, propertyName) ?? defaultValue;

    private sealed record ParsedLessonFile(
        JLPTLevel LevelCode,
        string? LevelName,
        string? ChapterTitle,
        int ChapterOrder,
        string LessonSlug,
        string LessonTitle,
        int LessonOrder,
        List<ParsedVocabularyItem> Vocabularies,
        List<ParsedGrammarItem> Grammars,
        List<ParsedReadingItem> Readings);

    private sealed record ParsedVocabularyItem(
        string Term,
        string Reading,
        string? Note,
        string MeaningsJson,
        string? ExamplesJson,
        string? OtherFormsJson,
        bool IsCommon,
        string? JlptTagsJson);

    private sealed record ParsedGrammarItem(
        string GrammarPoint,
        string? MeaningEn,
        string? DetailUrl,
        string? LevelHint,
        string? Explanation,
        string? UsageJson,
        string? ExamplesJson);

    private sealed record ParsedReadingItem(
        string Title,
        string Content,
        string? Translation,
        string? Url,
        string? LevelHint,
        string? Source);
}