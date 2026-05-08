using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Torisho.Application;
using Torisho.Application.DTOs.Quiz;
using Torisho.Application.Interfaces.Quiz;
using Torisho.Domain.Entities.ProgressDomain;
using Torisho.Domain.Entities.QuizDomain;
using Torisho.Domain.Enums;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Application.Services.Quiz;

public class PreparedQuizService : QuizGenerationServiceBase, IPreparedQuizService
{
    public PreparedQuizService(
        IDataContext context,
        IUnitOfWork unitOfWork,
        IQuizTemplateAiService quizTemplateAiService,
        IConfiguration configuration)
        : base(context, unitOfWork, quizTemplateAiService, configuration)
    {
    }

    public async Task<QuizDetailDto> GetLessonQuizAsync(Guid lessonId, QuizType? type = null, CancellationToken ct = default)
    {
        if (lessonId == Guid.Empty)
            throw new ArgumentException("lessonId is required", nameof(lessonId));

        var lessonExists = await Context.Set<Torisho.Domain.Entities.LearningDomain.Lesson>()
            .AsNoTracking()
            .AnyAsync(l => l.Id == lessonId, ct);

        if (!lessonExists)
            throw new KeyNotFoundException("Lesson not found");

        if (type.HasValue)
        {
            if (type.Value is not QuizType.Vocabulary and not QuizType.Grammar and not QuizType.Reading)
                throw new InvalidOperationException("Lesson quiz only supports Vocabulary, Grammar, or Reading");

            var byType = await GetLatestQuizByTargetAndTypeAsync(lessonId, type.Value, ct);
            if (byType is null)
                throw new KeyNotFoundException($"Lesson quiz has not been pre-generated for type '{type.Value}'");

            return ToQuizDetailDto(byType, isDaily: false, dailyDate: null);
        }

        var latestQuiz = await GetLatestLessonQuizAsync(lessonId, ct);
        if (latestQuiz is null)
            throw new KeyNotFoundException("Lesson quiz has not been pre-generated");

        return ToQuizDetailDto(latestQuiz, isDaily: false, dailyDate: null);
    }

    public async Task<QuizDetailDto> PreviewLessonQuizAsync(
        Guid lessonId,
        QuizType type,
        bool? useAiGeneration = null,
        string? templateMode = null,
        CancellationToken ct = default)
    {
        if (lessonId == Guid.Empty)
            throw new ArgumentException("lessonId is required", nameof(lessonId));

        if (type is not QuizType.Vocabulary and not QuizType.Grammar and not QuizType.Reading)
            throw new InvalidOperationException("Preview only supports Vocabulary, Grammar, or Reading");

        var lesson = await Context.Set<Torisho.Domain.Entities.LearningDomain.Lesson>()
            .AsNoTracking()
            .Include(l => l.VocabularyItems)
            .Include(l => l.GrammarItems)
            .Include(l => l.ReadingItems)
            .FirstOrDefaultAsync(l => l.Id == lessonId, ct);

        if (lesson is null)
            throw new KeyNotFoundException("Lesson not found");

        if (!HasLessonPool(lesson, type))
            throw new InvalidOperationException($"Lesson does not have enough content for {type}");

        var resolvedTemplateMode = ResolveTemplateMode(
            templateMode,
            ResolveTemplateMode(PregenerateTemplateMode, QuizTemplateMode.Diverse));

        var useAi = useAiGeneration ?? (QuizAiEnabled && QuizTemplateAiService.IsEnabled);

        var previewQuiz = await BuildLessonQuizAsync(
            lesson,
            type,
            resolvedTemplateMode,
            useAi,
            QuizGenerationPurpose.Lesson,
            ct);

        return ToQuizDetailDto(previewQuiz, isDaily: false, dailyDate: null);
    }

    public async Task<LessonQuizPregenerateResultDto> PregenerateLessonQuizzesAsync(QuizPregenerateRequest request, CancellationToken ct = default)
    {
        request ??= new QuizPregenerateRequest();

        var requestedTypes = (request.Types ?? Array.Empty<QuizType>())
            .Where(type => type is QuizType.Vocabulary or QuizType.Grammar or QuizType.Reading)
            .Distinct()
            .ToList();

        if (requestedTypes.Count == 0)
        {
            requestedTypes = new List<QuizType>
            {
                QuizType.Vocabulary,
                QuizType.Grammar,
                QuizType.Reading
            };
        }

        if (request.ChapterOrder.HasValue && request.ChapterOrder.Value <= 0)
            throw new ArgumentException("chapterOrder must be greater than 0", nameof(request));

        var templateMode = ResolveTemplateMode(
            request.TemplateMode,
            ResolveTemplateMode(PregenerateTemplateMode, QuizTemplateMode.Diverse));

        var useAi = request.UseAiGeneration ?? (QuizAiEnabled && QuizTemplateAiService.IsEnabled);

        var lessonsQuery = Context.Set<Torisho.Domain.Entities.LearningDomain.Lesson>()
            .AsNoTracking()
            .Include(l => l.VocabularyItems)
            .Include(l => l.GrammarItems)
            .Include(l => l.ReadingItems)
            .AsQueryable();

        if (request.LevelCode.HasValue)
        {
            var levelCode = request.LevelCode.Value;
            lessonsQuery = lessonsQuery.Where(l => l.SourceLevel == levelCode);
        }

        if (request.ChapterOrder.HasValue)
        {
            var chapterIds = await (
                from chapter in Context.Set<Torisho.Domain.Entities.LearningDomain.Chapter>().AsNoTracking()
                join level in Context.Set<Torisho.Domain.Entities.LearningDomain.Level>().AsNoTracking() on chapter.LevelId equals level.Id
                where chapter.Order == request.ChapterOrder.Value
                      && (!request.LevelCode.HasValue || level.Code == request.LevelCode.Value)
                select chapter.Id
            ).ToListAsync(ct);

            if (chapterIds.Count == 0)
            {
                return new LessonQuizPregenerateResultDto
                {
                    TotalLessons = 0,
                    CreatedCount = 0,
                    SkippedCount = 0,
                    FailedCount = 0,
                    Items = Array.Empty<LessonQuizPregenerateItemDto>()
                };
            }

            lessonsQuery = lessonsQuery.Where(l => chapterIds.Contains(l.ChapterId));
        }

        var lessons = await lessonsQuery
            .OrderBy(l => l.SourceLevel)
            .ThenBy(l => l.Order)
            .ToListAsync(ct);

        var items = new List<LessonQuizPregenerateItemDto>();

        foreach (var lesson in lessons)
        {
            foreach (var type in requestedTypes)
            {
                if (!HasLessonPool(lesson, type))
                {
                    items.Add(new LessonQuizPregenerateItemDto
                    {
                        LessonId = lesson.Id,
                        LessonSlug = lesson.Slug,
                        LessonTitle = lesson.Title,
                        Type = type,
                        Status = "skipped",
                        Message = $"Lesson does not have enough content for {type}"
                    });
                    continue;
                }

                var existing = await GetLatestQuizByTargetAndTypeAsync(lesson.Id, type, ct);
                if (existing is not null && !request.ForceRegenerate)
                {
                    items.Add(new LessonQuizPregenerateItemDto
                    {
                        LessonId = lesson.Id,
                        LessonSlug = lesson.Slug,
                        LessonTitle = lesson.Title,
                        Type = type,
                        Status = "skipped",
                        QuizId = existing.Id,
                        Message = "Existing quiz found"
                    });
                    continue;
                }

                try
                {
                    var quiz = await BuildLessonQuizAsync(
                        lesson,
                        type,
                        templateMode,
                        useAi,
                        QuizGenerationPurpose.Pregenerate,
                        ct);

                    await UnitOfWork.Quizzes.AddAsync(quiz, ct);
                    await UnitOfWork.SaveChangesAsync(ct);

                    items.Add(new LessonQuizPregenerateItemDto
                    {
                        LessonId = lesson.Id,
                        LessonSlug = lesson.Slug,
                        LessonTitle = lesson.Title,
                        Type = type,
                        Status = "created",
                        QuizId = quiz.Id,
                        Message = existing is not null ? "Created a newer quiz version" : null
                    });
                }
                catch (Exception ex)
                {
                    items.Add(new LessonQuizPregenerateItemDto
                    {
                        LessonId = lesson.Id,
                        LessonSlug = lesson.Slug,
                        LessonTitle = lesson.Title,
                        Type = type,
                        Status = "failed",
                        Message = ex.Message
                    });
                }
            }
        }

        return new LessonQuizPregenerateResultDto
        {
            TotalLessons = lessons.Count,
            CreatedCount = items.Count(i => i.Status == "created"),
            SkippedCount = items.Count(i => i.Status == "skipped"),
            FailedCount = items.Count(i => i.Status == "failed"),
            Items = items
        };
    }

    public async Task<QuizSubmitResultDto> SubmitQuizAsync(Guid userId, Guid quizId, SubmitQuizRequest request, CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("userId is required", nameof(userId));
        if (quizId == Guid.Empty)
            throw new ArgumentException("quizId is required", nameof(quizId));
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        var submitted = request.Answers
            .Where(a => a.QuestionId != Guid.Empty && a.SelectedOptionId != Guid.Empty)
            .GroupBy(a => a.QuestionId)
            .ToDictionary(g => g.Key, g => g.Last().SelectedOptionId);

        if (submitted.Count == 0)
            throw new InvalidOperationException("At least one answer is required");

        var quiz = await UnitOfWork.Quizzes.GetWithQuestionsAsync(quizId, ct);
        if (quiz is null)
            throw new KeyNotFoundException("Quiz not found");

        var orderedQuestions = quiz.Questions.OrderBy(q => q.Order).ToList();
        if (orderedQuestions.Count == 0)
            throw new InvalidOperationException("Quiz has no questions");

        if (submitted.Count != orderedQuestions.Count)
            throw new InvalidOperationException("All questions must be answered before submission");

        var attempt = new QuizAttempt(userId, quizId);
        var questionResults = new List<QuizQuestionResultDto>(orderedQuestions.Count);

        foreach (var question in orderedQuestions)
        {
            if (!submitted.TryGetValue(question.Id, out var selectedOptionId))
                throw new InvalidOperationException("Missing answer for one or more questions");

            var selectedOption = question.Options.FirstOrDefault(o => o.Id == selectedOptionId);
            if (selectedOption is null)
                throw new InvalidOperationException("Submitted answer does not belong to this quiz");

            var correctOption = question.GetCorrectOption();
            if (correctOption is null)
                throw new InvalidOperationException("Quiz data is invalid: missing correct option");

            var isCorrect = selectedOption.Id == correctOption.Id;
            var answer = new QuizAnswer(attempt.Id, question.Id, selectedOption.Id, isCorrect);
            attempt.AddAnswer(answer);

            var descriptor = QuizGenerationUtils.ParseQuestionDescriptor(
                question.Content,
                quiz.Type,
                isDaily: quiz.Type == QuizType.DailyChallenge);

            questionResults.Add(new QuizQuestionResultDto
            {
                QuestionId = question.Id,
                Skill = descriptor.Skill,
                Content = descriptor.DisplayContent,
                SelectedOptionId = selectedOption.Id,
                SelectedOptionText = selectedOption.OptionText,
                CorrectOptionId = correctOption.Id,
                CorrectOptionText = correctOption.OptionText,
                IsCorrect = isCorrect
            });
        }

        attempt.Submit();
        await UnitOfWork.QuizAttempts.AddAsync(attempt, ct);

        if (quiz.Type == QuizType.DailyChallenge)
        {
            await UpsertDailyQuizActivityAsync(userId, attempt.Score, ct);
        }

        var skillScores = questionResults
            .GroupBy(x => x.Skill)
            .Select(group =>
            {
                var total = group.Count();
                var correct = group.Count(item => item.IsCorrect);
                var score = total == 0 ? 0f : (float)Math.Round((correct / (double)total) * 100d, 2);

                return new QuizSkillScoreDto
                {
                    Skill = group.Key,
                    Total = total,
                    Correct = correct,
                    Score = score
                };
            })
            .OrderBy(x => x.Skill)
            .ToList();

        await UnitOfWork.SaveChangesAsync(ct);

        return new QuizSubmitResultDto
        {
            AttemptId = attempt.Id,
            QuizId = quizId,
            Score = attempt.Score,
            TotalQuestions = questionResults.Count,
            CorrectAnswers = questionResults.Count(x => x.IsCorrect),
            StartedAt = attempt.StartedAt,
            CompletedAt = attempt.CompletedAt ?? DateTime.UtcNow,
            SkillScores = skillScores,
            Questions = questionResults
        };
    }

    private async Task UpsertDailyQuizActivityAsync(Guid userId, float score, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var activity = await UnitOfWork.DailyActivities.GetByUserAndDateAsync(userId, today, ct);

        if (activity is null)
        {
            activity = new DailyActivities(userId, today);
            activity.RecordActivity("quiz");
            activity.CompleteDailyChallenge(score);
            activity.AddMinutes(5);
            await UnitOfWork.DailyActivities.AddAsync(activity, ct);
            return;
        }

        activity.RecordActivity("quiz");
        activity.CompleteDailyChallenge(score);
        activity.AddMinutes(5);
    }
}
