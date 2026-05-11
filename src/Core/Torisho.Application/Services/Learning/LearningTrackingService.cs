using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Application.DTOs.Learning;
using Torisho.Application.Interfaces.Learning;
using Torisho.Domain.Entities.LearningDomain;
using Torisho.Domain.Entities.ProgressDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Application.Services.Learning;

public sealed class LearningTrackingService : ILearningTrackingService
{
    private readonly IDataContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public LearningTrackingService(IDataContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task StartLessonAsync(Guid userId, string slug, CancellationToken ct = default)
    {
        var lesson = await GetLessonBySlugAsync(slug, ct);
        var (learningProgress, learningProgressCreated) = await GetOrCreateLearningProgressAsync(userId, lesson.Chapter!.LevelId, ct);
        await EnsureChapterProgressAsync(userId, lesson, ct);

        learningProgress.SetCurrentLesson(
            chapterId: lesson.ChapterId,
            lessonId: lesson.Id,
            lessonProgressPercent: learningProgress.CurrentLessonId == lesson.Id
                ? learningProgress.CurrentLessonProgressPercent
                : 0f,
            currentSection: learningProgress.CurrentLessonId == lesson.Id
                ? learningProgress.CurrentSection
                : "vocabulary");

        if (!learningProgressCreated)
            await _unitOfWork.LearningProgress.UpdateAsync(learningProgress, ct);

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UpdateLessonProgressAsync(Guid userId, string slug, LessonHeartbeatRequest request, CancellationToken ct = default)
    {
        var lesson = await GetLessonBySlugAsync(slug, ct);
        var (learningProgress, learningProgressCreated) = await GetOrCreateLearningProgressAsync(userId, lesson.Chapter!.LevelId, ct);
        var (chapterProgress, chapterProgressCreated) = await EnsureChapterProgressAsync(userId, lesson, ct);

        learningProgress.SetCurrentLesson(
            chapterId: lesson.ChapterId,
            lessonId: lesson.Id,
            lessonProgressPercent: request.LessonProgressPercent,
            currentSection: request.Section);

        if (request.ChapterProgressPercent.HasValue)
        {
            var totalLessonsInChapter = await _context.Set<Lesson>()
                .CountAsync(x => x.ChapterId == lesson.ChapterId, ct);

            chapterProgress.UpdatePercent(request.ChapterProgressPercent.Value, totalLessonsInChapter);

            if (!chapterProgressCreated)
                await _unitOfWork.ChapterProgress.UpdateAsync(chapterProgress, ct);
        }

        ApplyLevelSnapshot(learningProgress, request);

        if (!learningProgressCreated)
            await _unitOfWork.LearningProgress.UpdateAsync(learningProgress, ct);

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task CompleteLessonAsync(Guid userId, string slug, LessonHeartbeatRequest request, CancellationToken ct = default)
    {
        var lesson = await GetLessonBySlugAsync(slug, ct);
        var (learningProgress, learningProgressCreated) = await GetOrCreateLearningProgressAsync(userId, lesson.Chapter!.LevelId, ct);
        var (chapterProgress, chapterProgressCreated) = await EnsureChapterProgressAsync(userId, lesson, ct);

        learningProgress.SetCurrentLesson(
            chapterId: lesson.ChapterId,
            lessonId: lesson.Id,
            lessonProgressPercent: 100f,
            currentSection: request.Section ?? "completed");

        if (request.ChapterProgressPercent.HasValue)
        {
            var totalLessonsInChapter = await _context.Set<Lesson>()
                .CountAsync(x => x.ChapterId == lesson.ChapterId, ct);

            chapterProgress.UpdatePercent(request.ChapterProgressPercent.Value, totalLessonsInChapter);

            if (!chapterProgressCreated)
                await _unitOfWork.ChapterProgress.UpdateAsync(chapterProgress, ct);
        }

        ApplyLevelSnapshot(learningProgress, request with { LessonProgressPercent = 100f });

        await UpsertDailyActivitiesAsync(userId, lesson, ct);

        if (!learningProgressCreated)
            await _unitOfWork.LearningProgress.UpdateAsync(learningProgress, ct);

        await _unitOfWork.SaveChangesAsync(ct);
    }

    private async Task<Lesson> GetLessonBySlugAsync(string slug, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Slug is required", nameof(slug));

        var lesson = await _context.Set<Lesson>()
            .AsNoTracking()
            .Include(x => x.Chapter)
            .Include(x => x.VocabularyItems)
            .Include(x => x.GrammarItems)
            .Include(x => x.ReadingItems)
            .FirstOrDefaultAsync(x => x.Slug == slug.Trim(), ct);

        if (lesson is null || lesson.Chapter is null)
            throw new KeyNotFoundException("Lesson not found");

        return lesson;
    }

    private async Task<(LearningProgress Progress, bool Created)> GetOrCreateLearningProgressAsync(Guid userId, Guid levelId, CancellationToken ct)
    {
        var existing = await _unitOfWork.LearningProgress.GetByUserAndLevelAsync(userId, levelId, ct);
        if (existing is not null)
            return (existing, false);

        var progress = new LearningProgress(userId, levelId);
        await _unitOfWork.LearningProgress.AddAsync(progress, ct);
        return (progress, true);
    }

    private async Task<(ChapterProgress Progress, bool Created)> EnsureChapterProgressAsync(Guid userId, Lesson lesson, CancellationToken ct)
    {
        var existing = await _unitOfWork.ChapterProgress.GetByUserAndChapterAsync(userId, lesson.ChapterId, ct);
        if (existing is not null)
            return (existing, false);

        var totalLessonsInChapter = await _context.Set<Lesson>()
            .CountAsync(x => x.ChapterId == lesson.ChapterId, ct);

        var progress = new ChapterProgress(
            userId: userId,
            chapterId: lesson.ChapterId,
            levelId: lesson.Chapter!.LevelId,
            totalLessonCount: totalLessonsInChapter,
            isUnlocked: true);

        await _unitOfWork.ChapterProgress.AddAsync(progress, ct);
        return (progress, true);
    }

    private static void ApplyLevelSnapshot(LearningProgress learningProgress, LessonHeartbeatRequest request)
    {
        learningProgress.RefreshAggregates(
            vocabularyProgress: request.VocabularyProgress ?? learningProgress.VocabularyProgress,
            grammarProgress: request.GrammarProgress ?? learningProgress.GrammarProgress,
            readingProgress: request.ReadingProgress ?? learningProgress.ReadingProgress,
            listeningProgress: learningProgress.ListeningProgress,
            totalProgress: request.LevelProgressPercent ?? learningProgress.TotalProgress);
    }

    private async Task UpsertDailyActivitiesAsync(Guid userId, Lesson lesson, CancellationToken ct)
    {
        var nowLocal = DateTime.UtcNow.AddHours(7);
        var activityDate = DateOnly.FromDateTime(nowLocal);

        var daily = await _unitOfWork.DailyActivities.GetByUserAndDateAsync(userId, activityDate, ct);
        var dailyCreated = false;
        if (daily is null)
        {
            daily = new DailyActivities(userId, activityDate);
            await _unitOfWork.DailyActivities.AddAsync(daily, ct);
            dailyCreated = true;
        }

        daily.RecordActivity("vocabulary", lesson.VocabularyItems.Count);
        daily.RecordActivity("grammar", lesson.GrammarItems.Count);
        daily.RecordActivity("reading", lesson.ReadingItems.Count);
        daily.AddMinutes(Math.Max(1, lesson.VocabularyItems.Count + lesson.GrammarItems.Count + lesson.ReadingItems.Count));

        if (!dailyCreated)
            await _unitOfWork.DailyActivities.UpdateAsync(daily, ct);
    }
}
