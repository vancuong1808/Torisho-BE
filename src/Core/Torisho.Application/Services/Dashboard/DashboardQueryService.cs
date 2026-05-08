using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Application.DTOs.Dashboard;
using Torisho.Application.Interfaces.Dashboard;
using Torisho.Domain.Entities.DictionaryDomain;
using Torisho.Domain.Entities.LearningDomain;
using Torisho.Domain.Entities.ProgressDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Application.Services.Dashboard;

public sealed class DashboardQueryService : IDashboardQueryService
{
    private readonly IDataContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public DashboardQueryService(IDataContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<DashboardResponseDto> GetDashboardAsync(Guid userId, int? year = null, int? month = null, CancellationToken ct = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, ct);
        if (user is null)
            throw new KeyNotFoundException("User not found");

        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));
        var targetYear = year ?? today.Year;
        var targetMonth = month ?? today.Month;

        var monthStart = new DateOnly(targetYear, targetMonth, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        var levels = await _context.Set<Level>()
            .AsNoTracking()
            .OrderBy(x => x.Order)
            .ToListAsync(ct);

        var chapters = await _context.Set<Chapter>()
            .AsNoTracking()
            .ToListAsync(ct);

        var learningProgresses = (await _unitOfWork.LearningProgress.GetByUserAsync(userId, ct)).ToList();

        var chapterProgresses = await _context.Set<ChapterProgress>()
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync(ct);

        var monthActivities = (await _unitOfWork.DailyActivities
            .GetByUserAndRangeAsync(userId, monthStart, monthEnd, ct))
            .ToList();

        var streakActivities = (await _unitOfWork.DailyActivities
            .GetByUserAndRangeAsync(userId, today.AddDays(-365), today, ct))
            .ToList();

        var chapterCountByLevel = chapters
            .GroupBy(x => x.LevelId)
            .ToDictionary(x => x.Key, x => x.Count());

        var completedChapterCountByLevel = chapterProgresses
            .Where(x => x.CompletionPercent >= 100f)
            .GroupBy(x => x.LevelId)
            .ToDictionary(x => x.Key, x => x.Count());

        return new DashboardResponseDto
        {
            Profile = new DashboardProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl
            },
            Today = new DashboardTodayDto
            {
                Date = today,
                Timezone = "Asia/Saigon",
                DailyWord = await BuildDailyWordAsync(today, ct)
            },
            ProgressByLevel = levels.Select(level =>
            {
                var progress = learningProgresses.FirstOrDefault(x => x.LevelId == level.Id);

                return new DashboardLevelProgressDto
                {
                    LevelId = level.Id,
                    LevelCode = level.Code.ToString(),
                    LevelName = level.Name,
                    CompletionPercent = progress?.TotalProgress ?? 0f,
                    VocabularyProgress = progress?.VocabularyProgress ?? 0f,
                    GrammarProgress = progress?.GrammarProgress ?? 0f,
                    ReadingProgress = progress?.ReadingProgress ?? 0f,
                    CompletedChapters = completedChapterCountByLevel.TryGetValue(level.Id, out var completed) ? completed : 0,
                    TotalChapters = chapterCountByLevel.TryGetValue(level.Id, out var total) ? total : 0,
                    Status = progress?.GetStatus() ?? "NotStarted"
                };
            }).ToList(),
            Streak = BuildStreak(today, streakActivities),
            ContinueLearning = await BuildContinueLearningAsync(learningProgresses, ct),
            QuickStats = await BuildQuickStatsFromProgressSnapshotAsync(levels, learningProgresses, ct),
            Calendar = new DashboardCalendarDto
            {
                Year = targetYear,
                Month = targetMonth,
                StudyDates = monthActivities
                    .Select(x => x.ActivityDate)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList()
            }
        };
    }

    private async Task<DashboardContinueLearningDto?> BuildContinueLearningAsync(
        List<LearningProgress> learningProgresses,
        CancellationToken ct)
    {
        var latest = learningProgresses
            .Where(x => x.CurrentLessonId.HasValue)
            .OrderByDescending(x => x.LastUpdated)
            .FirstOrDefault();

        if (latest is null || !latest.CurrentLessonId.HasValue)
            return null;

        var lesson = await _context.Set<Lesson>()
            .AsNoTracking()
            .Include(x => x.Chapter!)
                .ThenInclude(c => c.Level)
            .FirstOrDefaultAsync(x => x.Id == latest.CurrentLessonId.Value, ct);

        if (lesson is null || lesson.Chapter is null || lesson.Chapter.Level is null)
            return null;

        return new DashboardContinueLearningDto
        {
            LevelId = lesson.Chapter.LevelId,
            LevelCode = lesson.Chapter.Level.Code.ToString(),
            ChapterId = lesson.ChapterId,
            ChapterTitle = lesson.Chapter.Title,
            LessonId = lesson.Id,
            LessonSlug = lesson.Slug,
            LessonTitle = lesson.Title,
            LessonProgressPercent = latest.CurrentLessonProgressPercent,
            CurrentSection = latest.CurrentSection,
            LastUpdated = latest.LastUpdated
        };
    }

    private async Task<DashboardQuickStatsDto> BuildQuickStatsFromProgressSnapshotAsync(
        List<Level> levels,
        List<LearningProgress> learningProgresses,
        CancellationToken ct)
    {
        var levelIds = levels.Select(x => x.Id).ToHashSet();

        var totalVocabularyByLevel = await _context.Set<LessonVocabularyItem>()
            .AsNoTracking()
            .Where(x => levelIds.Contains(x.Lesson!.Chapter!.LevelId))
            .GroupBy(x => x.Lesson!.Chapter!.LevelId)
            .Select(x => new { LevelId = x.Key, Count = x.Count() })
            .ToDictionaryAsync(x => x.LevelId, x => x.Count, ct);

        var totalGrammarByLevel = await _context.Set<LessonGrammarItem>()
            .AsNoTracking()
            .Where(x => levelIds.Contains(x.Lesson!.Chapter!.LevelId))
            .GroupBy(x => x.Lesson!.Chapter!.LevelId)
            .Select(x => new { LevelId = x.Key, Count = x.Count() })
            .ToDictionaryAsync(x => x.LevelId, x => x.Count, ct);

        var vocabTermsByLevel = await _context.Set<LessonVocabularyItem>()
            .AsNoTracking()
            .Where(x => levelIds.Contains(x.Lesson!.Chapter!.LevelId))
            .Select(x => new { LevelId = x.Lesson!.Chapter!.LevelId, x.Term })
            .ToListAsync(ct);

        var allKnownKanji = await _context.Set<Kanji>()
            .AsNoTracking()
            .Select(x => x.Character)
            .ToListAsync(ct);

        var knownKanjiSet = allKnownKanji.ToHashSet();

        var totalKanjiByLevel = vocabTermsByLevel
            .GroupBy(x => x.LevelId)
            .ToDictionary(
                x => x.Key,
                x =>
                {
                    var chars = x.SelectMany(v => v.Term.ToCharArray())
                        .Select(ch => ch.ToString())
                        .Distinct()
                        .Where(ch => knownKanjiSet.Contains(ch))
                        .Count();

                    return chars;
                });

        var vocabularyLearned = 0;
        var grammarLearned = 0;
        var kanjiLearned = 0;

        foreach (var progress in learningProgresses)
        {
            if (totalVocabularyByLevel.TryGetValue(progress.LevelId, out var totalVocabulary))
                vocabularyLearned += (int)Math.Round(totalVocabulary * progress.VocabularyProgress / 100f, MidpointRounding.AwayFromZero);

            if (totalGrammarByLevel.TryGetValue(progress.LevelId, out var totalGrammar))
                grammarLearned += (int)Math.Round(totalGrammar * progress.GrammarProgress / 100f, MidpointRounding.AwayFromZero);

            if (totalKanjiByLevel.TryGetValue(progress.LevelId, out var totalKanji))
                kanjiLearned += (int)Math.Round(totalKanji * progress.VocabularyProgress / 100f, MidpointRounding.AwayFromZero);
        }

        return new DashboardQuickStatsDto
        {
            VocabularyLearned = vocabularyLearned,
            GrammarLearned = grammarLearned,
            KanjiLearned = kanjiLearned
        };
    }

    private async Task<DashboardDailyWordDto?> BuildDailyWordAsync(DateOnly today, CancellationToken ct)
    {
        var total = await _context.Set<DictionaryEntry>().CountAsync(ct);
        if (total == 0)
            return null;

        var offset = Math.Abs(today.DayNumber) % total;

        var entry = await _context.Set<DictionaryEntry>()
            .AsNoTracking()
            .Include(x => x.Definition)
            .Include(x => x.KanjiForms)
            .Include(x => x.ReadingForms)
            .OrderBy(x => x.Id)
            .Skip(offset)
            .FirstOrDefaultAsync(ct);

        if (entry is null)
            return null;

        return new DashboardDailyWordDto
        {
            EntryId = entry.Id,
            Term = entry.KanjiForms.FirstOrDefault()?.KanjiText ?? entry.Keyword,
            Reading = entry.ReadingForms.FirstOrDefault()?.ReadingText ?? entry.Reading,
            Meaning = entry.Definition?.GlossText ?? string.Empty
        };
    }

    private static DashboardStreakDto BuildStreak(DateOnly today, List<DailyActivities> activities)
    {
        var studyDates = activities
            .Where(x => x.TotalMinutes > 0 ||
                        x.VocabularyCount > 0 ||
                        x.GrammarCount > 0 ||
                        x.ReadingCount > 0 ||
                        x.QuizCount > 0)
            .Select(x => x.ActivityDate)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var dateSet = studyDates.ToHashSet();
        var studiedToday = dateSet.Contains(today);

        var current = 0;
        var cursor = studiedToday ? today : today.AddDays(-1);

        while (dateSet.Contains(cursor))
        {
            current++;
            cursor = cursor.AddDays(-1);
        }

        var longest = 0;
        var running = 0;
        DateOnly? previous = null;

        foreach (var date in studyDates)
        {
            if (previous is null || date == previous.Value.AddDays(1))
                running++;
            else
                running = 1;

            if (running > longest)
                longest = running;

            previous = date;
        }

        return new DashboardStreakDto
        {
            Current = current,
            Longest = longest,
            StudiedToday = studiedToday
        };
    }
}
