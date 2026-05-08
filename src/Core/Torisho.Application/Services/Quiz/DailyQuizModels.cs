namespace Torisho.Application.Services.Quiz;

public enum QuizSkill
{
    Vocabulary,
    Kanji,
    Grammar,
    Reading
}

public enum QuizGenerationPurpose
{
    Lesson,
    Daily,
    Pregenerate
}

public enum QuizTemplateMode
{
    Classic,
    Diverse
}

public record VocabularySeed(string Term, string Reading, string Meaning, string ExampleSentence);

public record KanjiSeed(
    string Character,
    string Meaning,
    string Onyomi,
    string Kunyomi,
    IReadOnlyList<string> RelatedTerms);

public record GrammarSeed(string GrammarPoint, string Meaning, string ExampleSentence, string UsageHint);

public record ReadingSeed(string Title, string Content, string Summary);

public record DailyQuizBundle(
    IReadOnlyList<VocabularySeed> Vocabulary,
    IReadOnlyList<KanjiSeed> Kanji);

public record DailyGenerationContext(
    Torisho.Domain.Enums.JLPTLevel CurrentLevel,
    Dictionary<QuizSkill, double> Accuracy,
    Dictionary<QuizSkill, int> RecentMistakes,
    Dictionary<QuizSkill, double> ProgressDebt,
    DailyQuizBundle BaseBundle,
    IReadOnlyList<GrammarSeed> BaseGrammar,
    DailyQuizBundle ChallengeBundle,
    IReadOnlyList<GrammarSeed> ChallengeGrammar)
{
    public Dictionary<QuizSkill, double> MistakeRatio
    {
        get
        {
            var total = Math.Max(1, RecentMistakes.Values.Sum());
            return new Dictionary<QuizSkill, double>
            {
                [QuizSkill.Vocabulary] = RecentMistakes[QuizSkill.Vocabulary] / (double)total,
                [QuizSkill.Kanji] = RecentMistakes[QuizSkill.Kanji] / (double)total,
                [QuizSkill.Grammar] = RecentMistakes[QuizSkill.Grammar] / (double)total
            };
        }
    }
}

public record DailyAllocation(
    Dictionary<QuizSkill, int> BaseCounts,
    QuizSkill? ChallengeSkill,
    int ChallengeCount);
