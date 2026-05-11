using System.Text.Json;
using System.Text.RegularExpressions;
using Torisho.Domain.Entities.LearningDomain;
using Torisho.Domain.Enums;

namespace Torisho.Application.Services.Quiz;

public record ParsedQuestionDescriptor(
    string Skill,
    string Source,
    string Difficulty,
    string Topic,
    string DisplayContent);

public record OptionDraft(string Text, bool IsCorrect);

public record QuestionDraft(string Content, string CorrectOption, IReadOnlyList<string> Distractors);

internal static class QuizGenerationUtils
{
    private static readonly Regex ReadingNoiseLineRegex = new(
        @"(?im)^\s*(Question\s+\d+\s*:|JLPT\s+N[1-5]\s+Kanji\s+Lesson).*$",
        RegexOptions.Compiled);

    private static readonly Regex QuestionPrefixRegex = new(
        @"^\[(?<skill>[^\]]+)\](\[(?<source>[^\]]+)\])?\s*(?<content>.*)$",
        RegexOptions.Compiled);

    internal static IReadOnlyList<OptionDraft> BuildOptions(string correct, IReadOnlyList<string> distractors)
    {
        var normalizedCorrect = NormalizeText(correct);
        if (string.IsNullOrWhiteSpace(normalizedCorrect))
            normalizedCorrect = "Correct answer";

        var unique = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { normalizedCorrect };
        var selectedDistractors = new List<string>(capacity: 3);

        foreach (var candidate in distractors.Select(NormalizeText))
        {
            if (string.IsNullOrWhiteSpace(candidate))
                continue;
            if (unique.Add(candidate))
            {
                selectedDistractors.Add(candidate);
            }

            if (selectedDistractors.Count == 3)
                break;
        }

        var fallback = new[]
        {
            "None of the above",
            "Not applicable",
            "Another choice",
            "Unknown"
        };

        foreach (var fill in fallback)
        {
            if (selectedDistractors.Count == 3)
                break;

            if (unique.Add(fill))
                selectedDistractors.Add(fill);
        }

        var options = new List<OptionDraft>(4)
        {
            new(normalizedCorrect, true)
        };

        options.AddRange(selectedDistractors.Select(text => new OptionDraft(text, false)));

        var shuffled = options
            .OrderBy(_ => Random.Shared.Next())
            .Take(4)
            .ToList();

        if (shuffled.All(o => !o.IsCorrect))
        {
            shuffled[0] = new OptionDraft(normalizedCorrect, true);
        }

        return shuffled;
    }

    internal static List<VocabularySeed> BuildVocabularySeeds(IEnumerable<Lesson> lessons)
    {
        return lessons
            .SelectMany(lesson => lesson.VocabularyItems)
            .Select(item =>
            {
                var meaning = ExtractMeaning(item.MeaningsJson);
                var exampleSentence = ExtractExampleSentence(item.ExamplesJson);
                return new VocabularySeed(item.Term, item.Reading, meaning, exampleSentence);
            })
            .Where(seed => !string.IsNullOrWhiteSpace(seed.Term) && !string.IsNullOrWhiteSpace(seed.Meaning))
            .GroupBy(seed => seed.Term, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToList();
    }

    internal static List<GrammarSeed> BuildGrammarSeeds(IEnumerable<Lesson> lessons)
    {
        return lessons
            .SelectMany(lesson => lesson.GrammarItems)
            .Where(item => !ContainsNoise(item.GrammarPoint) &&
                           !ContainsNoise(item.MeaningEn) &&
                           !ContainsNoise(item.Explanation))
            .Select(item =>
            {
                var meaning = NormalizeText(item.MeaningEn);
                if (string.IsNullOrWhiteSpace(meaning))
                    meaning = NormalizeText(item.Explanation);

                var exampleSentence = ExtractExampleSentence(item.ExamplesJson);
                var usageHint = ExtractUsageHint(item.UsageJson, item.Explanation);

                return new GrammarSeed(item.GrammarPoint, meaning, exampleSentence, usageHint);
            })
            .Where(seed => !string.IsNullOrWhiteSpace(seed.GrammarPoint) && !string.IsNullOrWhiteSpace(seed.Meaning))
            .GroupBy(seed => seed.GrammarPoint, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToList();
    }

    internal static List<ReadingSeed> BuildReadingSeeds(IEnumerable<Lesson> lessons)
    {
        return lessons
            .SelectMany(lesson => lesson.ReadingItems)
            .Select(item =>
            {
                var cleaned = CleanReadingContent(item.Content);
                var summary = BuildReadingSummary(item.Translation, cleaned, item.Title);
                return new ReadingSeed(item.Title, cleaned, summary);
            })
            .Where(seed => !string.IsNullOrWhiteSpace(seed.Title) &&
                           !string.IsNullOrWhiteSpace(seed.Content) &&
                           seed.Content.Length >= 120 &&
                           !string.IsNullOrWhiteSpace(seed.Summary))
            .GroupBy(seed => seed.Title, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToList();
    }

    internal static ParsedQuestionDescriptor ParseQuestionDescriptor(string rawContent, QuizType quizType, bool isDaily)
    {
        var content = NormalizeText(rawContent);
        var match = QuestionPrefixRegex.Match(content);

        if (!match.Success)
        {
            var fallbackSkill = InferSkillLabel(quizType);
            return new ParsedQuestionDescriptor(
                fallbackSkill,
                isDaily ? "progress-review" : "lesson-fixed",
                "medium",
                fallbackSkill,
                content);
        }

        var skill = NormalizeText(match.Groups["skill"].Value);
        if (string.IsNullOrWhiteSpace(skill))
            skill = InferSkillLabel(quizType);

        var sourceTag = NormalizeText(match.Groups["source"].Value);
        var normalizedSource = sourceTag.Equals("challenge", StringComparison.OrdinalIgnoreCase)
            ? "light-challenge"
            : (isDaily ? "progress-review" : "lesson-fixed");

        var difficulty = sourceTag.Equals("challenge", StringComparison.OrdinalIgnoreCase)
            ? "hard"
            : "medium";

        var displayContent = NormalizeText(match.Groups["content"].Value);
        if (string.IsNullOrWhiteSpace(displayContent))
            displayContent = content;

        return new ParsedQuestionDescriptor(skill, normalizedSource, difficulty, skill, displayContent);
    }

    internal static string ExtractQuestionPrefix(string content, out string stem)
    {
        var normalized = NormalizeText(content);
        var match = QuestionPrefixRegex.Match(normalized);
        if (!match.Success)
        {
            stem = normalized;
            return string.Empty;
        }

        var skill = NormalizeText(match.Groups["skill"].Value);
        var source = NormalizeText(match.Groups["source"].Value);
        stem = NormalizeText(match.Groups["content"].Value);

        if (string.IsNullOrWhiteSpace(skill))
            return string.Empty;

        return string.IsNullOrWhiteSpace(source)
            ? $"[{skill}]"
            : $"[{skill}][{source}]";
    }

    internal static string ExtractMeaning(string meaningsJson)
    {
        if (string.IsNullOrWhiteSpace(meaningsJson))
            return string.Empty;

        try
        {
            using var doc = JsonDocument.Parse(meaningsJson);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                return string.Empty;

            foreach (var element in doc.RootElement.EnumerateArray())
            {
                if (element.ValueKind == JsonValueKind.String)
                {
                    var raw = NormalizeText(element.GetString());
                    if (!string.IsNullOrWhiteSpace(raw))
                        return raw;
                    continue;
                }

                if (element.ValueKind != JsonValueKind.Object)
                    continue;

                var vi = TryGetString(element, "vi");
                if (!string.IsNullOrWhiteSpace(vi))
                    return NormalizeText(vi);

                var en = TryGetString(element, "en");
                if (!string.IsNullOrWhiteSpace(en))
                    return NormalizeText(en);

                var meaning = TryGetString(element, "meaning");
                if (!string.IsNullOrWhiteSpace(meaning))
                    return NormalizeText(meaning);

                if (element.TryGetProperty("glosses", out var glosses)
                    && glosses.ValueKind == JsonValueKind.Array)
                {
                    foreach (var gloss in glosses.EnumerateArray())
                    {
                        if (gloss.ValueKind != JsonValueKind.String)
                            continue;

                        var rawGloss = NormalizeText(gloss.GetString());
                        if (!string.IsNullOrWhiteSpace(rawGloss))
                            return rawGloss;
                    }
                }

                var text = TryGetString(element, "text");
                if (!string.IsNullOrWhiteSpace(text))
                    return NormalizeText(text);
            }
        }
        catch (JsonException)
        {
            return string.Empty;
        }

        return string.Empty;
    }

    internal static string ExtractExampleSentence(string? examplesJson)
    {
        if (string.IsNullOrWhiteSpace(examplesJson))
            return string.Empty;

        try
        {
            using var doc = JsonDocument.Parse(examplesJson);
            return ExtractExampleSentenceCore(doc.RootElement);
        }
        catch (JsonException)
        {
            return string.Empty;
        }
    }

    internal static string ExtractUsageHint(string? usageJson, string? explanation)
    {
        var usage = ExtractExampleSentence(usageJson);
        if (!string.IsNullOrWhiteSpace(usage))
            return usage;

        return SafeExcerpt(explanation, 140);
    }

    internal static string CreateCloze(string sentence, string answerToken)
    {
        var normalizedSentence = NormalizeText(sentence);
        var normalizedAnswer = NormalizeText(answerToken);

        if (string.IsNullOrWhiteSpace(normalizedSentence) || string.IsNullOrWhiteSpace(normalizedAnswer))
            return string.Empty;

        var index = normalizedSentence.IndexOf(normalizedAnswer, StringComparison.Ordinal);
        if (index < 0)
            return string.Empty;

        return normalizedSentence.Remove(index, normalizedAnswer.Length).Insert(index, "___");
    }

    internal static string SafeExcerpt(string? value, int maxLength)
    {
        return TrimToLength(NormalizeText(value), maxLength);
    }

    internal static string CleanReadingContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return string.Empty;

        var normalized = content.Replace("\r\n", "\n");
        normalized = ReadingNoiseLineRegex.Replace(normalized, string.Empty);

        var lines = normalized
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(line => !ContainsNoise(line));

        return string.Join(" ", lines);
    }

    internal static string BuildReadingSummary(string? translation, string cleanedContent, string title)
    {
        var candidate = NormalizeText(translation);
        if (!string.IsNullOrWhiteSpace(candidate))
            return TrimToLength(ExtractFirstSentence(candidate), 220);

        if (!string.IsNullOrWhiteSpace(cleanedContent))
            return TrimToLength(ExtractFirstSentence(cleanedContent), 220);

        return NormalizeText(title);
    }

    internal static string ExtractFirstSentence(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var separators = new[] { '.', '!', '?', '。', '！', '？' };
        var index = input.IndexOfAny(separators);
        if (index <= 0)
            return NormalizeText(input);

        return NormalizeText(input[..(index + 1)]);
    }

    internal static bool TryMapSkill(QuizType type, out QuizSkill skill)
    {
        switch (type)
        {
            case QuizType.Vocabulary:
                skill = QuizSkill.Vocabulary;
                return true;
            case QuizType.Kanji:
                skill = QuizSkill.Kanji;
                return true;
            case QuizType.Grammar:
                skill = QuizSkill.Grammar;
                return true;
            case QuizType.Reading:
                skill = QuizSkill.Reading;
                return true;
            default:
                skill = default;
                return false;
        }
    }

    internal static bool TryMapSkillLabel(string value, out QuizSkill skill)
    {
        switch (NormalizeText(value).ToLowerInvariant())
        {
            case "vocabulary":
                skill = QuizSkill.Vocabulary;
                return true;
            case "kanji":
                skill = QuizSkill.Kanji;
                return true;
            case "grammar":
                skill = QuizSkill.Grammar;
                return true;
            case "reading":
                skill = QuizSkill.Reading;
                return true;
            default:
                skill = default;
                return false;
        }
    }

    internal static JLPTLevel? GetNextHarderLevel(JLPTLevel current)
    {
        return current switch
        {
            JLPTLevel.N5 => JLPTLevel.N4,
            JLPTLevel.N4 => JLPTLevel.N3,
            JLPTLevel.N3 => JLPTLevel.N2,
            JLPTLevel.N2 => JLPTLevel.N1,
            _ => null
        };
    }

    internal static double ClampToPercent(float value)
    {
        return Math.Clamp(value, 0f, 100f);
    }

    internal static double Clamp01(double value)
    {
        return Math.Clamp(value, 0d, 1d);
    }

    internal static string NormalizeText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
    }

    internal static string TrimToLength(string value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var normalized = value.Trim();
        if (normalized.Length <= maxLength)
            return normalized;

        return normalized[..maxLength];
    }

    internal static bool ContainsNoise(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return value.Contains("Learn Japanese grammar:", StringComparison.OrdinalIgnoreCase)
               || value.Contains("Patreon", StringComparison.OrdinalIgnoreCase);
    }

    private static string ExtractExampleSentenceCore(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                return NormalizeText(element.GetString());

            case JsonValueKind.Array:
                foreach (var child in element.EnumerateArray())
                {
                    var candidate = ExtractExampleSentenceCore(child);
                    if (!string.IsNullOrWhiteSpace(candidate))
                        return candidate;
                }

                return string.Empty;

            case JsonValueKind.Object:
            {
                var keyCandidates = new[]
                {
                    "ja", "jp", "japanese", "sentence", "text", "example", "value", "content", "vi", "en"
                };

                foreach (var key in keyCandidates)
                {
                    if (!element.TryGetProperty(key, out var prop))
                        continue;

                    var candidate = ExtractExampleSentenceCore(prop);
                    if (!string.IsNullOrWhiteSpace(candidate))
                        return candidate;
                }

                foreach (var property in element.EnumerateObject())
                {
                    var candidate = ExtractExampleSentenceCore(property.Value);
                    if (!string.IsNullOrWhiteSpace(candidate))
                        return candidate;
                }

                return string.Empty;
            }

            default:
                return string.Empty;
        }
    }

    private static string? TryGetString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
            return null;

        return property.ValueKind == JsonValueKind.String ? property.GetString() : null;
    }

    private static string InferSkillLabel(QuizType type)
    {
        return type switch
        {
            QuizType.Vocabulary => "Vocabulary",
            QuizType.Kanji => "Kanji",
            QuizType.Grammar => "Grammar",
            QuizType.Reading => "Reading",
            _ => "Mixed"
        };
    }
}
