using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Torisho.Application;
using Torisho.Application.DTOs.Quiz;
using Torisho.Application.Interfaces.Quiz;
using Torisho.Domain.Entities.LearningDomain;
using Torisho.Domain.Entities.QuizDomain;
using Torisho.Domain.Enums;
using Torisho.Domain.Interfaces.Repositories;
using QuizEntity = Torisho.Domain.Entities.QuizDomain.Quiz;
using static Torisho.Application.Services.Quiz.QuizGenerationUtils;

namespace Torisho.Application.Services.Quiz;

public abstract class QuizGenerationServiceBase
{
    protected const int DailyQuizQuestionCount = 10;
    protected const int LessonQuizQuestionCount = 10;
    protected const int RecentMistakeLookbackDays = 7;
    protected const int DailySkillActivityTarget = 14;

    protected readonly IDataContext Context;
    protected readonly IUnitOfWork UnitOfWork;
    protected readonly IQuizTemplateAiService QuizTemplateAiService;
    protected readonly bool QuizAiEnabled;
    protected readonly string DailyTemplateMode;
    protected readonly string PregenerateTemplateMode;

    protected QuizGenerationServiceBase(
        IDataContext context,
        IUnitOfWork unitOfWork,
        IQuizTemplateAiService quizTemplateAiService,
        IConfiguration configuration)
    {
        Context = context;
        UnitOfWork = unitOfWork;
        QuizTemplateAiService = quizTemplateAiService;
        QuizAiEnabled = configuration.GetValue<bool>("QuizAI:Enabled");
        DailyTemplateMode = configuration["QuizAI:DailyTemplateMode"] ?? "diverse";
        PregenerateTemplateMode = configuration["QuizAI:PregenerateTemplateMode"] ?? "diverse";
    }

    private protected static bool HasLessonPool(Lesson lesson, QuizType type)
    {
        return type switch
        {
            QuizType.Vocabulary => BuildVocabularySeeds(new[] { lesson }).Count > 0,
            QuizType.Grammar => BuildGrammarSeeds(new[] { lesson }).Count > 0,
            QuizType.Reading => BuildReadingSeeds(new[] { lesson }).Count > 0,
            _ => false
        };
    }

    private protected async Task<QuizEntity> BuildLessonQuizAsync(
        Lesson lesson,
        QuizType type,
        QuizTemplateMode templateMode,
        bool useAi,
        QuizGenerationPurpose purpose,
        CancellationToken ct)
    {
        var quiz = new QuizEntity(type, lesson.Id);

        switch (type)
        {
            case QuizType.Vocabulary:
            {
                var seeds = BuildVocabularySeeds(new[] { lesson });
                await AddVocabularyQuestionsAsync(quiz, seeds, LessonQuizQuestionCount, isChallenge: false, templateMode, useAi, purpose, ct);
                break;
            }
            case QuizType.Grammar:
            {
                var seeds = BuildGrammarSeeds(new[] { lesson });
                await AddGrammarQuestionsAsync(quiz, seeds, LessonQuizQuestionCount, isChallenge: false, templateMode, useAi, purpose, ct);
                break;
            }
            case QuizType.Reading:
            {
                var seeds = BuildReadingSeeds(new[] { lesson });
                await AddReadingQuestionsAsync(quiz, seeds, LessonQuizQuestionCount, isChallenge: false, templateMode, useAi, purpose, ct);
                break;
            }
            default:
                throw new InvalidOperationException("Unsupported lesson quiz type");
        }

        if (quiz.Questions.Count == 0)
            throw new InvalidOperationException("Unable to generate quiz from lesson content");

        return quiz;
    }

    private protected async Task AddVocabularyQuestionsAsync(
        QuizEntity quiz,
        IReadOnlyList<VocabularySeed> pool,
        int count,
        bool isChallenge,
        QuizTemplateMode templateMode,
        bool useAi,
        QuizGenerationPurpose purpose,
        CancellationToken ct)
    {
        if (count <= 0 || pool.Count == 0)
            return;

        var order = quiz.Questions.Count + 1;
        var variantOffset = Random.Shared.Next(0, 97);
        for (var i = 0; i < count; i++)
        {
            var seed = pool[(i + variantOffset) % pool.Count];
            var draft = CreateVocabularyDraft(seed, pool, isChallenge, templateMode, purpose, i + variantOffset);
            draft = await TryRewriteDraftWithAiAsync(draft, QuizSkill.Vocabulary, purpose, templateMode, useAi, ct);
            AddQuestion(quiz, draft, order++);
        }
    }

    private protected async Task AddKanjiQuestionsAsync(
        QuizEntity quiz,
        IReadOnlyList<KanjiSeed> pool,
        int count,
        bool isChallenge,
        QuizTemplateMode templateMode,
        bool useAi,
        QuizGenerationPurpose purpose,
        CancellationToken ct)
    {
        if (count <= 0 || pool.Count == 0)
            return;

        var order = quiz.Questions.Count + 1;
        var variantOffset = Random.Shared.Next(0, 97);
        for (var i = 0; i < count; i++)
        {
            var seed = pool[(i + variantOffset) % pool.Count];
            var draft = CreateKanjiDraft(seed, pool, isChallenge, templateMode, purpose, i + variantOffset);
            var anchors = seed.RelatedTerms.Append(seed.Character).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            draft = await TryRewriteDraftWithAiAsync(
                draft,
                QuizSkill.Kanji,
                purpose,
                templateMode,
                useAi,
                ct,
                anchorTerms: anchors,
                requiredKanji: seed.Character);
            AddQuestion(quiz, draft, order++);
        }
    }

    private protected async Task AddGrammarQuestionsAsync(
        QuizEntity quiz,
        IReadOnlyList<GrammarSeed> pool,
        int count,
        bool isChallenge,
        QuizTemplateMode templateMode,
        bool useAi,
        QuizGenerationPurpose purpose,
        CancellationToken ct)
    {
        if (count <= 0 || pool.Count == 0)
            return;

        var order = quiz.Questions.Count + 1;
        var variantOffset = Random.Shared.Next(0, 97);
        for (var i = 0; i < count; i++)
        {
            var seed = pool[(i + variantOffset) % pool.Count];
            var draft = CreateGrammarDraft(seed, pool, isChallenge, templateMode, purpose, i + variantOffset);
            draft = await TryRewriteDraftWithAiAsync(draft, QuizSkill.Grammar, purpose, templateMode, useAi, ct);
            AddQuestion(quiz, draft, order++);
        }
    }

    private protected async Task AddReadingQuestionsAsync(
        QuizEntity quiz,
        IReadOnlyList<ReadingSeed> pool,
        int count,
        bool isChallenge,
        QuizTemplateMode templateMode,
        bool useAi,
        QuizGenerationPurpose purpose,
        CancellationToken ct)
    {
        if (count <= 0 || pool.Count == 0)
            return;

        var order = quiz.Questions.Count + 1;
        var variantOffset = Random.Shared.Next(0, 97);
        for (var i = 0; i < count; i++)
        {
            var seed = pool[(i + variantOffset) % pool.Count];
            var draft = CreateReadingDraft(seed, pool, isChallenge, templateMode, purpose, i + variantOffset);
            draft = await TryRewriteDraftWithAiAsync(draft, QuizSkill.Reading, purpose, templateMode, useAi, ct);
            AddQuestion(quiz, draft, order++);
        }
    }

    private protected static void AddQuestion(QuizEntity quiz, QuestionDraft draft, int order)
    {
        var question = new Question(quiz.Id, TrimToLength(draft.Content, 1000), order);
        var options = BuildOptions(draft.CorrectOption, draft.Distractors);

        foreach (var option in options)
        {
            question.AddOption(new QuestionOption(question.Id, TrimToLength(option.Text, 500), option.IsCorrect));
        }

        quiz.AddQuestion(question);
    }

    private protected static QuestionDraft CreateVocabularyDraft(
        VocabularySeed seed,
        IReadOnlyList<VocabularySeed> pool,
        bool isChallenge,
        QuizTemplateMode templateMode,
        QuizGenerationPurpose purpose,
        int variantHint)
    {
        var prefix = isChallenge ? "[Vocabulary][challenge]" : "[Vocabulary]";

        var meaningDistractors = pool
            .Where(x => !string.Equals(x.Meaning, seed.Meaning, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Meaning)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var readingDistractors = pool
            .Where(x => !string.Equals(x.Reading, seed.Reading, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Reading)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (templateMode == QuizTemplateMode.Classic)
        {
            return new QuestionDraft(
                $"{prefix} What is the best meaning of '{seed.Term}' ({seed.Reading})?",
                seed.Meaning,
                meaningDistractors);
        }

        var termDistractors = pool
            .Where(x => !string.Equals(x.Term, seed.Term, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Term)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var hasReading = !string.IsNullOrWhiteSpace(seed.Reading) && readingDistractors.Count > 0;

        if (purpose == QuizGenerationPurpose.Daily)
        {
            var variantTotal = hasReading ? 3 : 2;
            var variant = Math.Abs(variantHint) % variantTotal;
            return variant switch
            {
                0 => new QuestionDraft(
                    $"{prefix} Which option best translates '{seed.Term}'?",
                    seed.Meaning,
                    meaningDistractors),

                1 => new QuestionDraft(
                    $"{prefix} Choose the best word for this meaning: '{seed.Meaning}'.",
                    seed.Term,
                    termDistractors),

                _ => new QuestionDraft(
                    $"{prefix} What is the hiragana reading of '{seed.Term}'?",
                    seed.Reading,
                    readingDistractors)
            };
        }

        var clozeSentence = CreateCloze(seed.ExampleSentence, seed.Term);
        var hasCloze = !string.IsNullOrWhiteSpace(clozeSentence);

        var variants = new List<QuestionDraft>
        {
            new(
                $"{prefix} 「{seed.Term}」の意味として最も適切なものを選びなさい。",
                seed.Meaning,
                meaningDistractors),

            new(
                $"{prefix} 次の意味に最も合う語を選びなさい: 「{seed.Meaning}」",
                seed.Term,
                termDistractors)
        };

        if (hasReading)
        {
            variants.Add(new QuestionDraft(
                $"{prefix} 「{seed.Term}」の読み方として正しいものを選びなさい。",
                seed.Reading,
                readingDistractors));
        }

        if (hasCloze)
        {
            variants.Add(new QuestionDraft(
                $"{prefix} 次の文の（___）に最も適切な語を選びなさい。{clozeSentence}",
                seed.Term,
                termDistractors));

            variants.Add(new QuestionDraft(
                $"{prefix} 次の文中の「{seed.Term}」に最も近い意味を選びなさい。{SafeExcerpt(seed.ExampleSentence, 120)}",
                seed.Meaning,
                meaningDistractors));
        }

        var index = Math.Abs(variantHint) % variants.Count;
        return variants[index];
    }

    private protected static QuestionDraft CreateGrammarDraft(
        GrammarSeed seed,
        IReadOnlyList<GrammarSeed> pool,
        bool isChallenge,
        QuizTemplateMode templateMode,
        QuizGenerationPurpose purpose,
        int variantHint)
    {
        var prefix = isChallenge ? "[Grammar][challenge]" : "[Grammar]";

        var meaningDistractors = pool
            .Where(x => !string.Equals(x.Meaning, seed.Meaning, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Meaning)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (templateMode == QuizTemplateMode.Classic)
        {
            return new QuestionDraft(
                $"{prefix} What meaning matches grammar point '{seed.GrammarPoint}'?",
                seed.Meaning,
                meaningDistractors);
        }

        var grammarPointDistractors = pool
            .Where(x => !string.Equals(x.GrammarPoint, seed.GrammarPoint, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.GrammarPoint)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (purpose == QuizGenerationPurpose.Daily)
        {
            var variant = Math.Abs(variantHint) % 3;
            return variant switch
            {
                0 => new QuestionDraft(
                    $"{prefix} What meaning matches grammar point '{seed.GrammarPoint}'?",
                    seed.Meaning,
                    meaningDistractors),

                1 => new QuestionDraft(
                    $"{prefix} Which grammar pattern best fits this meaning: '{seed.Meaning}'?",
                    seed.GrammarPoint,
                    grammarPointDistractors),

                _ => new QuestionDraft(
                    $"{prefix} Fill the blank with the most suitable grammar point: [___] {seed.Meaning}.",
                    seed.GrammarPoint,
                    grammarPointDistractors)
            };
        }

        var clozeSentence = CreateCloze(seed.ExampleSentence, seed.GrammarPoint);
        var hasCloze = !string.IsNullOrWhiteSpace(clozeSentence);

        var variants = new List<QuestionDraft>
        {
            new(
                $"{prefix} 文法項目「{seed.GrammarPoint}」の意味として最も適切なものを選びなさい。",
                seed.Meaning,
                meaningDistractors),

            new(
                $"{prefix} 次の意味に最も合う文型を選びなさい: 「{seed.Meaning}」",
                seed.GrammarPoint,
                grammarPointDistractors)
        };

        if (hasCloze)
        {
            variants.Add(new QuestionDraft(
                $"{prefix} 次の文の（___）に最も適切な文型を選びなさい。{clozeSentence}",
                seed.GrammarPoint,
                grammarPointDistractors));
        }

        if (!string.IsNullOrWhiteSpace(seed.UsageHint))
        {
            variants.Add(new QuestionDraft(
                $"{prefix} 次の説明に最も合う文型を選びなさい。{SafeExcerpt(seed.UsageHint, 120)}",
                seed.GrammarPoint,
                grammarPointDistractors));
        }

        var index = Math.Abs(variantHint) % variants.Count;
        return variants[index];
    }

    private protected static QuestionDraft CreateKanjiDraft(
        KanjiSeed seed,
        IReadOnlyList<KanjiSeed> pool,
        bool isChallenge,
        QuizTemplateMode templateMode,
        QuizGenerationPurpose purpose,
        int variantHint)
    {
        var prefix = isChallenge ? "[Kanji][challenge]" : "[Kanji]";

        var meaningDistractors = pool
            .Where(x => !string.Equals(x.Meaning, seed.Meaning, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Meaning)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var readingDistractors = pool
            .SelectMany(x => new[] { x.Onyomi, x.Kunyomi })
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(NormalizeText)
            .Where(x => !string.Equals(x, seed.Onyomi, StringComparison.OrdinalIgnoreCase)
                     && !string.Equals(x, seed.Kunyomi, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var kanjiDistractors = pool
            .Where(x => !string.Equals(x.Character, seed.Character, StringComparison.Ordinal))
            .Select(x => x.Character)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        var relatedTerm = seed.RelatedTerms.FirstOrDefault() ?? string.Empty;

        if (templateMode == QuizTemplateMode.Classic)
        {
            return new QuestionDraft(
                $"{prefix} Which meaning best matches kanji '{seed.Character}'?",
                seed.Meaning,
                meaningDistractors);
        }

        var hasReading = !string.IsNullOrWhiteSpace(seed.Kunyomi) || !string.IsNullOrWhiteSpace(seed.Onyomi);
        var preferredReading = !string.IsNullOrWhiteSpace(seed.Kunyomi) ? seed.Kunyomi : seed.Onyomi;

        if (purpose == QuizGenerationPurpose.Daily)
        {
            var variant = Math.Abs(variantHint) % (hasReading ? 3 : 2);
            return variant switch
            {
                0 => new QuestionDraft(
                    $"{prefix} Which meaning best matches kanji '{seed.Character}' used in daily word '{relatedTerm}'?",
                    seed.Meaning,
                    meaningDistractors),
                1 => new QuestionDraft(
                    $"{prefix} Which kanji matches this meaning: '{seed.Meaning}'?",
                    seed.Character,
                    kanjiDistractors),
                _ => new QuestionDraft(
                    $"{prefix} What is the best reading for kanji '{seed.Character}' in daily word '{relatedTerm}'?",
                    preferredReading,
                    readingDistractors)
            };
        }

        var variants = new List<QuestionDraft>
        {
            new(
                $"{prefix} 漢字「{seed.Character}」の意味として最も適切なものを選びなさい。",
                seed.Meaning,
                meaningDistractors),
            new(
                $"{prefix} 次の意味に最も合う漢字を選びなさい: 「{seed.Meaning}」",
                seed.Character,
                kanjiDistractors)
        };

        if (hasReading)
        {
            variants.Add(new QuestionDraft(
                $"{prefix} 漢字「{seed.Character}」の読み方として最も適切なものを選びなさい。",
                preferredReading,
                readingDistractors));
        }

        var index = Math.Abs(variantHint) % variants.Count;
        return variants[index];
    }

    private protected static QuestionDraft CreateReadingDraft(
        ReadingSeed seed,
        IReadOnlyList<ReadingSeed> pool,
        bool isChallenge,
        QuizTemplateMode templateMode,
        QuizGenerationPurpose purpose,
        int variantHint)
    {
        var excerpt = TrimToLength(seed.Content, 180);
        var prefix = isChallenge ? "[Reading][challenge]" : "[Reading]";

        var summaryDistractors = pool
            .Where(x => !string.Equals(x.Summary, seed.Summary, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Summary)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (templateMode == QuizTemplateMode.Classic)
        {
            return new QuestionDraft(
                $"{prefix} Based on passage '{seed.Title}', what is the best summary? Excerpt: {excerpt}",
                seed.Summary,
                summaryDistractors);
        }

        var titleDistractors = pool
            .Where(x => !string.Equals(x.Title, seed.Title, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Title)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (purpose == QuizGenerationPurpose.Daily)
        {
            var variant = Math.Abs(variantHint) % 3;
            return variant switch
            {
                0 => new QuestionDraft(
                    $"{prefix} Based on passage '{seed.Title}', what is the best summary? Excerpt: {excerpt}",
                    seed.Summary,
                    summaryDistractors),

                1 => new QuestionDraft(
                    $"{prefix} Which title best matches this excerpt: {excerpt}",
                    seed.Title,
                    titleDistractors),

                _ => new QuestionDraft(
                    $"{prefix} Which statement is most supported by this passage excerpt: {excerpt}",
                    seed.Summary,
                    summaryDistractors)
            };
        }

        var variants = new List<QuestionDraft>
        {
            new(
                $"{prefix} 次の文章の内容として最も適切なものを選びなさい。本文: {excerpt}",
                seed.Summary,
                summaryDistractors),

            new(
                $"{prefix} 本文の主題として最も適切なものを選びなさい。本文: {excerpt}",
                seed.Summary,
                summaryDistractors),

            new(
                $"{prefix} この文章に最も合うタイトルを選びなさい。本文: {excerpt}",
                seed.Title,
                titleDistractors),

            new(
                $"{prefix} 筆者の意図として最も近いものを選びなさい。本文: {excerpt}",
                seed.Summary,
                summaryDistractors)
        };

        var index = Math.Abs(variantHint) % variants.Count;
        return variants[index];
    }

    private protected async Task<QuestionDraft> TryRewriteDraftWithAiAsync(
        QuestionDraft draft,
        QuizSkill skill,
        QuizGenerationPurpose purpose,
        QuizTemplateMode templateMode,
        bool useAi,
        CancellationToken ct,
        IReadOnlyList<string>? anchorTerms = null,
        string? requiredGrammarPoint = null,
        string? requiredKanji = null)
    {
        if (!useAi || !QuizTemplateAiService.IsEnabled)
            return draft;

        var prefix = ExtractQuestionPrefix(draft.Content, out var stem);
        if (string.IsNullOrWhiteSpace(stem))
            return draft;

        var rewritten = await QuizTemplateAiService.TryRewriteQuestionAsync(
            new QuizAiRewriteRequest
            {
                Skill = skill.ToString(),
                Purpose = purpose.ToString(),
                QuizType = MapQuizTypeLabel(purpose),
                TemplateMode = templateMode.ToString().ToLowerInvariant(),
                Level = ExtractJlptLevel(stem),
                LearningContent = BuildLearningContentSnapshot(draft),
                WeakSkills = purpose == QuizGenerationPurpose.Daily ? skill.ToString() : string.Empty,
                Mistakes = purpose == QuizGenerationPurpose.Daily ? skill.ToString() : string.Empty,
                CompletedTopics = "N/A",
                CurrentQuestion = stem,
                CorrectOption = draft.CorrectOption,
                RequiredGrammarPoint = requiredGrammarPoint ?? string.Empty,
                RequiredKanji = requiredKanji ?? string.Empty,
                AnchorTerms = anchorTerms ?? Array.Empty<string>(),
                Distractors = draft.Distractors.Take(3).ToList()
            },
            ct);

        if (string.IsNullOrWhiteSpace(rewritten))
            return draft;

        var content = string.IsNullOrWhiteSpace(prefix)
            ? rewritten
            : $"{prefix} {rewritten}";

        return draft with { Content = content };
    }

    private protected static string MapQuizTypeLabel(QuizGenerationPurpose purpose)
    {
        return purpose switch
        {
            QuizGenerationPurpose.Daily => "daily",
            QuizGenerationPurpose.Pregenerate => "chapter",
            QuizGenerationPurpose.Lesson => "lesson",
            _ => "lesson"
        };
    }

    private protected static string ExtractJlptLevel(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return "N/A";

        var match = Regex.Match(content, @"\bN[1-5]\b", RegexOptions.IgnoreCase);
        return match.Success ? match.Value.ToUpperInvariant() : "N/A";
    }

    private protected static string BuildLearningContentSnapshot(QuestionDraft draft)
    {
        var parts = new List<string>
        {
            $"Stem: {NormalizeText(draft.Content)}",
            $"Correct: {NormalizeText(draft.CorrectOption)}"
        };

        if (draft.Distractors.Count > 0)
        {
            parts.Add($"Distractors: {string.Join(" | ", draft.Distractors.Where(x => !string.IsNullOrWhiteSpace(x)).Take(3))}");
        }

        return TrimToLength(string.Join("; ", parts), 700);
    }

    private protected async Task<QuizEntity?> GetLatestQuizByTargetAndTypeAsync(Guid targetContentId, QuizType type, CancellationToken ct)
    {
        var quizId = await Context.Set<QuizEntity>()
            .AsNoTracking()
            .Where(q => q.TargetContentId == targetContentId && q.Type == type)
            .OrderByDescending(q => q.CreatedAt)
            .Select(q => q.Id)
            .FirstOrDefaultAsync(ct);

        if (quizId == Guid.Empty)
            return null;

        return await UnitOfWork.Quizzes.GetWithQuestionsAsync(quizId, ct);
    }

    private protected async Task<QuizEntity?> GetLatestLessonQuizAsync(Guid lessonId, CancellationToken ct)
    {
        var lessonTypes = new[] { QuizType.Vocabulary, QuizType.Grammar, QuizType.Reading };

        var quizId = await Context.Set<QuizEntity>()
            .AsNoTracking()
            .Where(q => q.TargetContentId == lessonId && lessonTypes.Contains(q.Type))
            .OrderByDescending(q => q.CreatedAt)
            .Select(q => q.Id)
            .FirstOrDefaultAsync(ct);

        if (quizId == Guid.Empty)
            return null;

        return await UnitOfWork.Quizzes.GetWithQuestionsAsync(quizId, ct);
    }

    private protected async Task<QuizEntity?> GetDailyQuizByDateAsync(Guid userId, DateOnly date, CancellationToken ct)
    {
        var start = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var end = start.AddDays(1);

        var quizId = await Context.Set<QuizEntity>()
            .AsNoTracking()
            .Where(q => q.Type == QuizType.DailyChallenge &&
                        q.TargetContentId == userId &&
                        q.CreatedAt >= start &&
                        q.CreatedAt < end)
            .OrderByDescending(q => q.CreatedAt)
            .Select(q => q.Id)
            .FirstOrDefaultAsync(ct);

        if (quizId == Guid.Empty)
            return null;

        return await UnitOfWork.Quizzes.GetWithQuestionsAsync(quizId, ct);
    }

    private protected static QuizTemplateMode ResolveTemplateMode(string? value, QuizTemplateMode fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
            return fallback;

        return value.Equals("classic", StringComparison.OrdinalIgnoreCase)
            ? QuizTemplateMode.Classic
            : QuizTemplateMode.Diverse;
    }

    protected static QuizDetailDto ToQuizDetailDto(QuizEntity quiz, bool isDaily, DateOnly? dailyDate)
    {
        var questions = quiz.Questions
            .OrderBy(q => q.Order)
            .Select(q =>
            {
                var descriptor = ParseQuestionDescriptor(q.Content, quiz.Type, isDaily);

                return new QuizQuestionDto
                {
                    QuestionId = q.Id,
                    Order = q.Order,
                    Skill = descriptor.Skill,
                    Source = descriptor.Source,
                    Difficulty = descriptor.Difficulty,
                    Topic = descriptor.Topic,
                    Content = descriptor.DisplayContent,
                    Options = q.Options
                        .Select(o => new QuizOptionDto
                        {
                            OptionId = o.Id,
                            Text = o.OptionText
                        })
                        .ToList()
                };
            })
            .ToList();

        var estimatedMinutes = Math.Max(3, (int)Math.Ceiling(questions.Count * 0.7));

        return new QuizDetailDto
        {
            QuizId = quiz.Id,
            Type = quiz.Type,
            TargetContentId = quiz.TargetContentId,
            Title = isDaily ? "Daily Smart Quiz" : $"Lesson {quiz.Type} Quiz",
            IsDaily = isDaily,
            DailyDate = dailyDate,
            EstimatedMinutes = estimatedMinutes,
            Tip = isDaily
                ? "Questions are weighted by your weak areas and recent mistakes."
                : "This quiz is generated from the selected lesson content.",
            QuestionCount = questions.Count,
            Questions = questions
        };
    }
}
