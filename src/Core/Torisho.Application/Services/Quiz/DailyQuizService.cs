using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Torisho.Application;
using Torisho.Application.DTOs.Quiz;
using Torisho.Application.Interfaces.Quiz;
using Torisho.Domain.Entities.DictionaryDomain;
using Torisho.Domain.Entities.LearningDomain;
using Torisho.Domain.Entities.ProgressDomain;
using Torisho.Domain.Enums;
using Torisho.Domain.Interfaces.Repositories;
using QuizEntity = Torisho.Domain.Entities.QuizDomain.Quiz;
using static Torisho.Application.Services.Quiz.QuizGenerationUtils;

namespace Torisho.Application.Services.Quiz;

public class DailyQuizService : QuizGenerationServiceBase, IDailyQuizService
{
    private const int DailyBundleVocabularyTarget = 8;

    private readonly IMemoryCache _dailyQuizCache;
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> DailyQuizGenerationLocks = new();

    public DailyQuizService(
        IDataContext context,
        IUnitOfWork unitOfWork,
        IQuizTemplateAiService quizTemplateAiService,
        IMemoryCache dailyQuizCache,
        IConfiguration configuration)
        : base(context, unitOfWork, quizTemplateAiService, configuration)
    {
        _dailyQuizCache = dailyQuizCache;
    }

    public async Task<DailyQuizDto> GetDailyQuizAsync(Guid userId, CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("userId is required", nameof(userId));

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var cacheKey = BuildDailyCacheKey(userId, today);

        if (_dailyQuizCache.TryGetValue<Guid>(cacheKey, out var cachedQuizId) && cachedQuizId != Guid.Empty)
        {
            var cachedQuiz = await UnitOfWork.Quizzes.GetWithQuestionsAsync(cachedQuizId, ct);
            if (cachedQuiz is not null)
            {
                return new DailyQuizDto
                {
                    Date = today,
                    IsCached = true,
                    Quiz = ToQuizDetailDto(cachedQuiz, isDaily: true, dailyDate: today)
                };
            }

            _dailyQuizCache.Remove(cacheKey);
        }

        var gate = DailyQuizGenerationLocks.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(ct);
        try
        {
            if (_dailyQuizCache.TryGetValue<Guid>(cacheKey, out cachedQuizId) && cachedQuizId != Guid.Empty)
            {
                var cachedQuiz = await UnitOfWork.Quizzes.GetWithQuestionsAsync(cachedQuizId, ct);
                if (cachedQuiz is not null)
                {
                    return new DailyQuizDto
                    {
                        Date = today,
                        IsCached = true,
                        Quiz = ToQuizDetailDto(cachedQuiz, isDaily: true, dailyDate: today)
                    };
                }

                _dailyQuizCache.Remove(cacheKey);
            }

            var existingDaily = await GetDailyQuizByDateAsync(userId, today, ct);
            if (existingDaily is not null)
            {
                CacheDailyQuiz(cacheKey, existingDaily.Id);
                return new DailyQuizDto
                {
                    Date = today,
                    IsCached = true,
                    Quiz = ToQuizDetailDto(existingDaily, isDaily: true, dailyDate: today)
                };
            }

            var generationContext = await BuildDailyGenerationContextAsync(userId, ct);
            var allocation = BuildDailyAllocation(generationContext);
            var dailyTemplateMode = ResolveTemplateMode(DailyTemplateMode, QuizTemplateMode.Diverse);
            var useAi = QuizAiEnabled && QuizTemplateAiService.IsEnabled;
            var quiz = await BuildDailyQuizAsync(userId, generationContext, allocation, dailyTemplateMode, useAi, ct);

            await UnitOfWork.Quizzes.AddAsync(quiz, ct);
            await UnitOfWork.SaveChangesAsync(ct);

            CacheDailyQuiz(cacheKey, quiz.Id);

            return new DailyQuizDto
            {
                Date = today,
                IsCached = false,
                Quiz = ToQuizDetailDto(quiz, isDaily: true, dailyDate: today)
            };
        }
        finally
        {
            gate.Release();
        }
    }

    private async Task<QuizEntity> BuildDailyQuizAsync(
        Guid userId,
        DailyGenerationContext context,
        DailyAllocation allocation,
        QuizTemplateMode templateMode,
        bool useAi,
        CancellationToken ct)
    {
        var quiz = new QuizEntity(QuizType.DailyChallenge, userId);

        await AddVocabularyQuestionsAsync(
            quiz,
            context.BaseBundle.Vocabulary,
            allocation.BaseCounts[QuizSkill.Vocabulary],
            isChallenge: false,
            templateMode,
            useAi,
            QuizGenerationPurpose.Daily,
            ct);

        await AddKanjiQuestionsAsync(
            quiz,
            context.BaseBundle.Kanji,
            allocation.BaseCounts[QuizSkill.Kanji],
            isChallenge: false,
            templateMode,
            useAi,
            QuizGenerationPurpose.Daily,
            ct);

        await AddDailyGrammarQuestionsAsync(
            quiz,
            context.BaseGrammar,
            context.BaseBundle,
            allocation.BaseCounts[QuizSkill.Grammar],
            isChallenge: false,
            templateMode,
            useAi,
            ct);

        if (allocation.ChallengeCount > 0 && allocation.ChallengeSkill.HasValue)
        {
            switch (allocation.ChallengeSkill.Value)
            {
                case QuizSkill.Vocabulary:
                    await AddVocabularyQuestionsAsync(
                        quiz,
                        context.ChallengeBundle.Vocabulary,
                        allocation.ChallengeCount,
                        isChallenge: true,
                        templateMode,
                        useAi,
                        QuizGenerationPurpose.Daily,
                        ct);
                    break;
                case QuizSkill.Kanji:
                    await AddKanjiQuestionsAsync(
                        quiz,
                        context.ChallengeBundle.Kanji,
                        allocation.ChallengeCount,
                        isChallenge: true,
                        templateMode,
                        useAi,
                        QuizGenerationPurpose.Daily,
                        ct);
                    break;
                case QuizSkill.Grammar:
                    await AddDailyGrammarQuestionsAsync(
                        quiz,
                        context.ChallengeGrammar,
                        context.ChallengeBundle,
                        allocation.ChallengeCount,
                        isChallenge: true,
                        templateMode,
                        useAi,
                        ct);
                    break;
            }
        }

        if (quiz.Questions.Count < DailyQuizQuestionCount)
        {
            var missing = DailyQuizQuestionCount - quiz.Questions.Count;
            await AddVocabularyQuestionsAsync(quiz, context.BaseBundle.Vocabulary, missing, false, templateMode, useAi, QuizGenerationPurpose.Daily, ct);
        }

        if (quiz.Questions.Count < DailyQuizQuestionCount)
        {
            var missing = DailyQuizQuestionCount - quiz.Questions.Count;
            await AddKanjiQuestionsAsync(quiz, context.BaseBundle.Kanji, missing, false, templateMode, useAi, QuizGenerationPurpose.Daily, ct);
        }

        if (quiz.Questions.Count < DailyQuizQuestionCount)
        {
            var missing = DailyQuizQuestionCount - quiz.Questions.Count;
            await AddDailyGrammarQuestionsAsync(quiz, context.BaseGrammar, context.BaseBundle, missing, false, templateMode, useAi, ct);
        }

        if (quiz.Questions.Count == 0)
            throw new InvalidOperationException("Unable to generate daily quiz from current content pool");

        if (quiz.Questions.Count > DailyQuizQuestionCount)
        {
            var ordered = quiz.Questions.OrderBy(q => q.Order).Take(DailyQuizQuestionCount).ToList();
            var rebuilt = new QuizEntity(QuizType.DailyChallenge, userId);
            var order = 1;
            foreach (var question in ordered)
            {
                var draft = new QuestionDraft(
                    question.Content,
                    question.GetCorrectOption()?.OptionText ?? "Correct answer",
                    question.Options.Where(o => !o.IsCorrect).Select(o => o.OptionText).ToList());

                AddQuestion(rebuilt, draft, order++);
            }

            return rebuilt;
        }

        return quiz;
    }

    private async Task AddDailyGrammarQuestionsAsync(
        QuizEntity quiz,
        IReadOnlyList<GrammarSeed> grammarPool,
        DailyQuizBundle bundle,
        int count,
        bool isChallenge,
        QuizTemplateMode templateMode,
        bool useAi,
        CancellationToken ct)
    {
        if (count <= 0 || grammarPool.Count == 0 || (bundle.Vocabulary.Count == 0 && bundle.Kanji.Count == 0))
            return;

        var order = quiz.Questions.Count + 1;
        var variantOffset = Random.Shared.Next(0, 97);
        for (var i = 0; i < count; i++)
        {
            var grammarSeed = grammarPool[(i + variantOffset) % grammarPool.Count];
            var vocabAnchor = bundle.Vocabulary.Count == 0 ? null : bundle.Vocabulary[(i + variantOffset) % bundle.Vocabulary.Count];
            var kanjiAnchor = bundle.Kanji.Count == 0 ? null : bundle.Kanji[(i + variantOffset) % bundle.Kanji.Count];
            var draft = CreateBoundGrammarDraft(grammarSeed, grammarPool, vocabAnchor, kanjiAnchor, isChallenge, templateMode, i + variantOffset);
            var anchors = BuildGrammarAnchorTerms(vocabAnchor, kanjiAnchor);

            draft = await TryRewriteDraftWithAiAsync(
                draft,
                QuizSkill.Grammar,
                QuizGenerationPurpose.Daily,
                templateMode,
                useAi,
                ct,
                anchorTerms: anchors,
                requiredGrammarPoint: grammarSeed.GrammarPoint,
                requiredKanji: kanjiAnchor?.Character);

            AddQuestion(quiz, draft, order++);
        }
    }

    private async Task<DailyGenerationContext> BuildDailyGenerationContextAsync(Guid userId, CancellationToken ct)
    {
        var levelInfo = await GetCurrentLevelSignalAsync(userId, ct);
        var unlockedChapters = (await UnitOfWork.ChapterProgress.GetUnlockedChaptersAsync(userId, ct))
            .Where(cp => cp.LevelId == levelInfo.LevelId)
            .Select(cp => cp.ChapterId)
            .Distinct()
            .ToHashSet();

        var baseLessons = await LoadLessonsByLevelAsync(levelInfo.LevelId, unlockedChapters, ct);
        var baseVocabulary = BuildVocabularySeeds(baseLessons);
        var baseGrammar = BuildGrammarSeeds(baseLessons);
        var baseBundle = await BuildDailyBundleAsync(baseVocabulary, ct);

        var challengeLevelCode = GetNextHarderLevel(levelInfo.LevelCode);
        var challengeLessons = new List<Lesson>();
        if (challengeLevelCode.HasValue)
        {
            var challengeLevelId = await Context.Set<Level>()
                .AsNoTracking()
                .Where(l => l.Code == challengeLevelCode.Value)
                .Select(l => l.Id)
                .FirstOrDefaultAsync(ct);

            if (challengeLevelId != Guid.Empty)
            {
                challengeLessons = await LoadLessonsByLevelAsync(challengeLevelId, Array.Empty<Guid>(), ct);
            }
        }

        var challengeVocabulary = BuildVocabularySeeds(challengeLessons);
        var challengeGrammar = BuildGrammarSeeds(challengeLessons);
        var challengeBundle = await BuildDailyBundleAsync(challengeVocabulary, ct);

        if (baseBundle.Vocabulary.Count == 0 && challengeBundle.Vocabulary.Count > 0)
            baseBundle = challengeBundle;
        if (baseGrammar.Count == 0 && challengeGrammar.Count > 0)
            baseGrammar = challengeGrammar;

        if (baseBundle.Vocabulary.Count == 0 && baseBundle.Kanji.Count == 0 && baseGrammar.Count == 0)
            throw new InvalidOperationException("No quiz-ready content found for daily quiz generation");

        var mistakes = await GetRecentMistakesBySkillAsync(userId, ct);
        var progressDebt = await GetProgressDebtBySkillAsync(userId, levelInfo.LevelId, ct);

        return new DailyGenerationContext(
            levelInfo.LevelCode,
            levelInfo.Accuracy,
            mistakes,
            progressDebt,
            baseBundle,
            baseGrammar,
            challengeBundle,
            challengeGrammar);
    }

    private async Task<DailyQuizBundle> BuildDailyBundleAsync(
        IReadOnlyList<VocabularySeed> vocabularyPool,
        CancellationToken ct)
    {
        if (vocabularyPool.Count == 0)
            return new DailyQuizBundle(Array.Empty<VocabularySeed>(), Array.Empty<KanjiSeed>());

        var selectedVocabulary = SelectVocabularyBundle(vocabularyPool);
        var kanji = await BuildKanjiSeedsAsync(selectedVocabulary, ct);

        return new DailyQuizBundle(selectedVocabulary, kanji);
    }

    private DailyAllocation BuildDailyAllocation(DailyGenerationContext context)
    {
        var skills = new[] { QuizSkill.Vocabulary, QuizSkill.Kanji, QuizSkill.Grammar };
        var hasPool = new Dictionary<QuizSkill, bool>
        {
            [QuizSkill.Vocabulary] = context.BaseBundle.Vocabulary.Count > 0,
            [QuizSkill.Kanji] = context.BaseBundle.Kanji.Count > 0,
            [QuizSkill.Grammar] = context.BaseGrammar.Count > 0
        };

        var scores = new Dictionary<QuizSkill, double>();
        foreach (var skill in skills)
        {
            var a = 1d - (context.Accuracy[skill] / 100d);
            var m = context.MistakeRatio[skill];
            var p = context.ProgressDebt[skill];
            scores[skill] = (0.50d * a) + (0.30d * m) + (0.20d * p);
        }

        var readiness = skills.Average(skill => context.Accuracy[skill]) / 100d;
        var challengeRatio = readiness switch
        {
            < 0.55d => 0d,
            < 0.75d => 0.10d,
            _ => 0.20d
        };

        var challengeCount = Math.Min((int)Math.Round(DailyQuizQuestionCount * challengeRatio), (int)Math.Floor(DailyQuizQuestionCount * 0.20d));
        var baseCount = DailyQuizQuestionCount - challengeCount;

        var eligible = skills.Where(skill => hasPool[skill]).ToList();
        if (eligible.Count == 0)
            throw new InvalidOperationException("No eligible skill pool for daily quiz");

        var minimum = eligible.ToDictionary(
            skill => skill,
            skill => (context.Accuracy[skill] <= 75d || context.RecentMistakes[skill] > 0) ? 1 : 0);

        if (minimum.Values.Sum() > baseCount)
        {
            foreach (var skill in eligible)
                minimum[skill] = 0;

            foreach (var skill in eligible.OrderByDescending(skill => scores[skill]).Take(baseCount))
                minimum[skill] = 1;
        }

        var remaining = baseCount - minimum.Values.Sum();
        var scoreSum = Math.Max(eligible.Sum(skill => scores[skill]), 1e-9d);
        var raw = eligible.ToDictionary(skill => skill, skill => remaining * scores[skill] / scoreSum);
        var allocation = eligible.ToDictionary(skill => skill, skill => minimum[skill] + (int)Math.Floor(raw[skill]));

        var leftover = baseCount - allocation.Values.Sum();
        foreach (var skill in eligible
                     .OrderByDescending(skill => raw[skill] - Math.Floor(raw[skill]))
                     .ThenByDescending(skill => scores[skill])
                     .Take(leftover))
        {
            allocation[skill] += 1;
        }

        var challengeSkill = ResolveChallengeSkill(context);
        if (challengeCount > 0 && challengeSkill is null)
            challengeCount = 0;

        foreach (var skill in skills)
        {
            if (!allocation.ContainsKey(skill))
                allocation[skill] = 0;
        }

        return new DailyAllocation(allocation, challengeSkill, challengeCount);
    }

    private async Task<(Guid LevelId, JLPTLevel LevelCode, Dictionary<QuizSkill, double> Accuracy)> GetCurrentLevelSignalAsync(Guid userId, CancellationToken ct)
    {
        var progress = await (
            from lp in Context.Set<LearningProgress>().AsNoTracking()
            join level in Context.Set<Level>().AsNoTracking() on lp.LevelId equals level.Id
            where lp.UserId == userId
            orderby lp.LastUpdated descending
            select new
            {
                lp.LevelId,
                level.Code,
                lp.VocabularyProgress,
                lp.GrammarProgress
            }).FirstOrDefaultAsync(ct);

        if (progress is not null)
        {
            var vocabularyAccuracy = ClampToPercent(progress.VocabularyProgress);
            var grammarAccuracy = ClampToPercent(progress.GrammarProgress);

            return (
                progress.LevelId,
                progress.Code,
                new Dictionary<QuizSkill, double>
                {
                    [QuizSkill.Vocabulary] = vocabularyAccuracy,
                    [QuizSkill.Kanji] = vocabularyAccuracy,
                    [QuizSkill.Grammar] = grammarAccuracy
                });
        }

        var defaultLevel = await Context.Set<Level>()
            .AsNoTracking()
            .OrderBy(l => l.Order)
            .Select(l => new { l.Id, l.Code })
            .FirstOrDefaultAsync(ct);

        if (defaultLevel is null)
            throw new InvalidOperationException("No JLPT levels configured in database");

        return (
            defaultLevel.Id,
            defaultLevel.Code,
            new Dictionary<QuizSkill, double>
            {
                [QuizSkill.Vocabulary] = 50d,
                [QuizSkill.Kanji] = 50d,
                [QuizSkill.Grammar] = 50d
            });
    }

    private async Task<Dictionary<QuizSkill, int>> GetRecentMistakesBySkillAsync(Guid userId, CancellationToken ct)
    {
        var fromDate = DateTime.UtcNow.AddDays(-RecentMistakeLookbackDays);
        var rows = await (
            from answer in Context.Set<Torisho.Domain.Entities.QuizDomain.QuizAnswer>().AsNoTracking()
            join attempt in Context.Set<Torisho.Domain.Entities.QuizDomain.QuizAttempt>().AsNoTracking() on answer.AttemptId equals attempt.Id
            join question in Context.Set<Torisho.Domain.Entities.QuizDomain.Question>().AsNoTracking() on answer.QuestionId equals question.Id
            join quiz in Context.Set<QuizEntity>().AsNoTracking() on question.QuizId equals quiz.Id
            where attempt.UserId == userId && attempt.StartedAt >= fromDate && !answer.IsCorrect
            select new { quiz.Type, question.Content }
        ).ToListAsync(ct);

        var mistakes = new Dictionary<QuizSkill, int>
        {
            [QuizSkill.Vocabulary] = 0,
            [QuizSkill.Kanji] = 0,
            [QuizSkill.Grammar] = 0
        };

        foreach (var row in rows)
        {
            var resolvedSkill = ResolveSkillFromQuizHistory(row.Type, row.Content);
            if (!resolvedSkill.HasValue || !mistakes.ContainsKey(resolvedSkill.Value))
                continue;

            mistakes[resolvedSkill.Value]++;
        }

        return mistakes;
    }

    private async Task<Dictionary<QuizSkill, double>> GetProgressDebtBySkillAsync(Guid userId, Guid levelId, CancellationToken ct)
    {
        var chapterProgresses = (await UnitOfWork.ChapterProgress.GetByUserAndLevelAsync(userId, levelId, ct))
            .Where(cp => cp.IsUnlocked)
            .ToList();

        var chapterDebt = chapterProgresses.Count == 0
            ? 0.50d
            : chapterProgresses.Average(cp => Clamp01(1d - (cp.CompletionPercent / 100d)));

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var fromDate = today.AddDays(-RecentMistakeLookbackDays + 1);
        var activity = await UnitOfWork.DailyActivities.GetByUserAndRangeAsync(userId, fromDate, today, ct);

        var vocabularyCount = activity.Sum(a => a.VocabularyCount);
        var grammarCount = activity.Sum(a => a.GrammarCount);

        var vocabularyDebt = 1d - Math.Min(vocabularyCount / (double)DailySkillActivityTarget, 1d);
        var grammarDebt = 1d - Math.Min(grammarCount / (double)DailySkillActivityTarget, 1d);

        return new Dictionary<QuizSkill, double>
        {
            [QuizSkill.Vocabulary] = Clamp01((0.60d * chapterDebt) + (0.40d * vocabularyDebt)),
            [QuizSkill.Kanji] = Clamp01((0.60d * chapterDebt) + (0.40d * vocabularyDebt)),
            [QuizSkill.Grammar] = Clamp01((0.60d * chapterDebt) + (0.40d * grammarDebt))
        };
    }

    private async Task<List<Lesson>> LoadLessonsByLevelAsync(Guid levelId, IReadOnlyCollection<Guid> preferredChapterIds, CancellationToken ct)
    {
        var chapterIds = await Context.Set<Chapter>()
            .AsNoTracking()
            .Where(c => c.LevelId == levelId)
            .OrderBy(c => c.Order)
            .Select(c => c.Id)
            .ToListAsync(ct);

        if (chapterIds.Count == 0)
            return new List<Lesson>();

        var selectedChapterIds = preferredChapterIds.Count > 0
            ? chapterIds.Where(preferredChapterIds.Contains).ToList()
            : chapterIds;

        if (selectedChapterIds.Count == 0)
            selectedChapterIds = chapterIds;

        return await Context.Set<Lesson>()
            .AsNoTracking()
            .Include(l => l.VocabularyItems)
            .Include(l => l.GrammarItems)
            .Where(l => selectedChapterIds.Contains(l.ChapterId))
            .OrderBy(l => l.Order)
            .ToListAsync(ct);
    }

    private async Task<IReadOnlyList<KanjiSeed>> BuildKanjiSeedsAsync(
        IReadOnlyList<VocabularySeed> selectedVocabulary,
        CancellationToken ct)
    {
        var kanjiChars = selectedVocabulary
            .SelectMany(vocabulary => vocabulary.Term.EnumerateRunes())
            .Where(IsLikelyKanjiRune)
            .Select(rune => rune.ToString())
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (kanjiChars.Count == 0)
            return Array.Empty<KanjiSeed>();

        var entities = await Context.Set<Kanji>()
            .AsNoTracking()
            .Where(kanji => kanjiChars.Contains(kanji.Character))
            .ToListAsync(ct);

        return entities
            .Select(entity =>
            {
                var relatedTerms = selectedVocabulary
                    .Where(vocabulary => vocabulary.Term.Contains(entity.Character, StringComparison.Ordinal))
                    .Select(vocabulary => vocabulary.Term)
                    .Distinct(StringComparer.Ordinal)
                    .Take(3)
                    .ToList();

                return new KanjiSeed(
                    entity.Character,
                    ExtractKanjiMeaning(entity.MeaningsJson),
                    NormalizeText(entity.Onyomi),
                    NormalizeText(entity.Kunyomi),
                    relatedTerms);
            })
            .Where(seed => !string.IsNullOrWhiteSpace(seed.Character)
                        && (!string.IsNullOrWhiteSpace(seed.Meaning)
                            || !string.IsNullOrWhiteSpace(seed.Onyomi)
                            || !string.IsNullOrWhiteSpace(seed.Kunyomi)))
            .OrderBy(seed => seed.Character, StringComparer.Ordinal)
            .ToList();
    }

    private static IReadOnlyList<VocabularySeed> SelectVocabularyBundle(IReadOnlyList<VocabularySeed> vocabularyPool)
    {
        var prioritized = vocabularyPool
            .OrderByDescending(seed => ContainsKanji(seed.Term))
            .ThenByDescending(seed => !string.IsNullOrWhiteSpace(seed.ExampleSentence))
            .ThenBy(_ => Random.Shared.Next())
            .Take(Math.Min(DailyBundleVocabularyTarget, vocabularyPool.Count))
            .ToList();

        if (prioritized.Count > 0)
            return prioritized;

        return vocabularyPool.Take(Math.Min(DailyBundleVocabularyTarget, vocabularyPool.Count)).ToList();
    }

    private static QuestionDraft CreateBoundGrammarDraft(
        GrammarSeed seed,
        IReadOnlyList<GrammarSeed> pool,
        VocabularySeed? vocabAnchor,
        KanjiSeed? kanjiAnchor,
        bool isChallenge,
        QuizTemplateMode templateMode,
        int variantHint)
    {
        var prefix = isChallenge ? "[Grammar][challenge]" : "[Grammar]";
        var anchorTerm = vocabAnchor?.Term ?? kanjiAnchor?.Character ?? "the daily topic";
        var anchorMeaning = vocabAnchor?.Meaning ?? kanjiAnchor?.Meaning ?? "the daily topic";

        var grammarPointDistractors = pool
            .Where(x => !string.Equals(x.GrammarPoint, seed.GrammarPoint, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.GrammarPoint)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var meaningDistractors = pool
            .Where(x => !string.Equals(x.Meaning, seed.Meaning, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Meaning)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (templateMode == QuizTemplateMode.Classic)
        {
            return new QuestionDraft(
                $"{prefix} Using daily word '{anchorTerm}', which grammar pattern best matches '{seed.Meaning}'?",
                seed.GrammarPoint,
                grammarPointDistractors);
        }

        var variant = Math.Abs(variantHint) % 3;
        return variant switch
        {
            0 => new QuestionDraft(
                $"{prefix} Using daily word '{anchorTerm}' ({anchorMeaning}), which grammar pattern best expresses '{seed.Meaning}'?",
                seed.GrammarPoint,
                grammarPointDistractors),
            1 => new QuestionDraft(
                $"{prefix} Which meaning best matches grammar point '{seed.GrammarPoint}' when talking about daily word '{anchorTerm}'?",
                seed.Meaning,
                meaningDistractors),
            _ => new QuestionDraft(
                $"{prefix} Build a natural sentence around daily word '{anchorTerm}' with the grammar point that means '{seed.Meaning}'. Which grammar point should be used?",
                seed.GrammarPoint,
                grammarPointDistractors)
        };
    }

    private static IReadOnlyList<string> BuildGrammarAnchorTerms(VocabularySeed? vocabAnchor, KanjiSeed? kanjiAnchor)
    {
        var anchors = new List<string>();

        if (vocabAnchor is not null)
        {
            anchors.Add(vocabAnchor.Term);
            if (!string.IsNullOrWhiteSpace(vocabAnchor.Reading))
                anchors.Add(vocabAnchor.Reading);
            if (!string.IsNullOrWhiteSpace(vocabAnchor.Meaning))
                anchors.Add(vocabAnchor.Meaning);
        }

        if (kanjiAnchor is not null)
        {
            anchors.Add(kanjiAnchor.Character);
            if (!string.IsNullOrWhiteSpace(kanjiAnchor.Meaning))
                anchors.Add(kanjiAnchor.Meaning);
        }

        return anchors
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static QuizSkill? ResolveChallengeSkill(DailyGenerationContext context)
    {
        var candidates = new List<(QuizSkill Skill, int PoolSize)>
        {
            (QuizSkill.Vocabulary, context.ChallengeBundle.Vocabulary.Count),
            (QuizSkill.Kanji, context.ChallengeBundle.Kanji.Count),
            (QuizSkill.Grammar, context.ChallengeGrammar.Count)
        }
        .Where(x => x.PoolSize > 0)
        .ToList();

        if (candidates.Count == 0)
            return null;

        return candidates
            .OrderByDescending(x => context.Accuracy[x.Skill])
            .ThenByDescending(x => x.PoolSize)
            .First()
            .Skill;
    }

    private static QuizSkill? ResolveSkillFromQuizHistory(QuizType type, string questionContent)
    {
        if (type == QuizType.DailyChallenge)
        {
            var descriptor = ParseQuestionDescriptor(questionContent, type, isDaily: true);
            return TryMapSkillLabel(descriptor.Skill, out var dailySkill) ? dailySkill : null;
        }

        return TryMapSkill(type, out var skill) ? skill : null;
    }

    private static string BuildDailyCacheKey(Guid userId, DateOnly date)
    {
        return $"quiz:daily:{userId:N}:{date:yyyyMMdd}";
    }

    private void CacheDailyQuiz(string cacheKey, Guid quizId)
    {
        if (quizId == Guid.Empty)
            return;

        var now = DateTime.UtcNow;
        var midnight = now.Date.AddDays(1);
        var ttl = midnight - now;
        if (ttl <= TimeSpan.Zero)
            ttl = TimeSpan.FromMinutes(1);

        _dailyQuizCache.Set(cacheKey, quizId, ttl);
    }

    private static bool ContainsKanji(string value)
    {
        return value.EnumerateRunes().Any(IsLikelyKanjiRune);
    }

    private static bool IsLikelyKanjiRune(Rune rune)
    {
        return rune.Value is >= 0x4E00 and <= 0x9FFF
            or >= 0x3400 and <= 0x4DBF
            or >= 0xF900 and <= 0xFAFF;
    }

    private static string ExtractKanjiMeaning(string meaningsJson)
    {
        if (string.IsNullOrWhiteSpace(meaningsJson))
            return string.Empty;

        try
        {
            using var document = JsonDocument.Parse(meaningsJson);
            if (document.RootElement.ValueKind != JsonValueKind.Array)
                return string.Empty;

            foreach (var item in document.RootElement.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String)
                {
                    var value = NormalizeText(item.GetString());
                    if (!string.IsNullOrWhiteSpace(value))
                        return value;
                }
            }
        }
        catch (JsonException)
        {
            return string.Empty;
        }

        return string.Empty;
    }
}
