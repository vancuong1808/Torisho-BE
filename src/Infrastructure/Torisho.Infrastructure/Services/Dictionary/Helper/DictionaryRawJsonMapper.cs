using System.Text.Json;
using Torisho.Application.DTOs.Dictionary;

namespace Torisho.Infrastructure.Services.Dictionary;

internal static class DictionaryRawJsonMapper
{
    internal static WordSchemaDto? TryParseWord(string? rawJson, string? keywordLower = null)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
            return null;

        try
        {
            using var doc = JsonDocument.Parse(rawJson);
            var root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
                return null;

            var (kanji, kana, isCommon) = ExtractPrimaryForms(root);
            var primaryMeaning = ExtractPrimaryMeaning(root, keywordLower);

            // Id will be filled from DB row.
            return new WordSchemaDto(
                Id: Guid.Empty,
                Kanji: kanji,
                Kana: kana ?? string.Empty,
                IsCommon: isCommon,
                MatchedReading: null,
                PrimaryMeaning: primaryMeaning);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static (string? Kanji, string? Kana, bool IsCommon) ExtractPrimaryForms(JsonElement wordObj)
    {
        string? kanjiText = null;
        string? kanaText = null;
        var isCommon = false;

        if (wordObj.TryGetProperty("kanji", out var kanjiList) && kanjiList.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in kanjiList.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object)
                    continue;

                if (kanjiText is null && item.TryGetProperty("text", out var textEl) && textEl.ValueKind == JsonValueKind.String)
                    kanjiText = textEl.GetString();

                if (item.TryGetProperty("common", out var commonEl) && commonEl.ValueKind == JsonValueKind.True)
                    isCommon = true;
            }
        }

        if (wordObj.TryGetProperty("kana", out var kanaList) && kanaList.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in kanaList.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object)
                    continue;

                if (kanaText is null && item.TryGetProperty("text", out var textEl) && textEl.ValueKind == JsonValueKind.String)
                    kanaText = textEl.GetString();

                if (item.TryGetProperty("common", out var commonEl) && commonEl.ValueKind == JsonValueKind.True)
                    isCommon = true;
            }
        }

        if (kanaText is null)
            kanaText = string.Empty;

        return (kanjiText, kanaText, isCommon);
    }

    private static string ExtractPrimaryMeaning(JsonElement wordObj, string? keywordLower)
    {
        var glosses = ExtractEnglishGlosses(wordObj);
        if (glosses.Count == 0)
            return string.Empty;

        var normalizedKeyword = keywordLower?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedKeyword))
            return glosses[0];

        var bestScore = int.MinValue;
        var bestLength = int.MaxValue;
        string? bestGloss = null;

        foreach (var gloss in glosses)
        {
            var score = ScoreGloss(gloss, normalizedKeyword);
            if (score <= 0)
                continue;

            if (score > bestScore || (score == bestScore && gloss.Length < bestLength))
            {
                bestScore = score;
                bestLength = gloss.Length;
                bestGloss = gloss;
            }
        }

        return bestGloss ?? glosses[0];
    }

    private static List<string> ExtractEnglishGlosses(JsonElement wordObj)
    {
        var result = new List<string>();

        if (!wordObj.TryGetProperty("sense", out var senseList) || senseList.ValueKind != JsonValueKind.Array)
            return result;

        foreach (var sense in senseList.EnumerateArray())
        {
            if (sense.ValueKind != JsonValueKind.Object)
                continue;

            if (!sense.TryGetProperty("gloss", out var glossList) || glossList.ValueKind != JsonValueKind.Array)
                continue;

            foreach (var g in glossList.EnumerateArray())
            {
                if (g.ValueKind == JsonValueKind.String)
                {
                    var s = g.GetString();
                    if (!string.IsNullOrWhiteSpace(s))
                        result.Add(s);
                    continue;
                }

                if (g.ValueKind != JsonValueKind.Object)
                    continue;

                var lang = g.TryGetProperty("lang", out var langEl) && langEl.ValueKind == JsonValueKind.String
                    ? langEl.GetString()
                    : null;

                if (!string.IsNullOrWhiteSpace(lang) && !string.Equals(lang, "eng", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (g.TryGetProperty("text", out var textEl) && textEl.ValueKind == JsonValueKind.String)
                {
                    var s = textEl.GetString();
                    if (!string.IsNullOrWhiteSpace(s))
                        result.Add(s);
                }
            }
        }

        return result;
    }

    private static int ScoreGloss(string gloss, string keywordLower)
    {
        var normalizedGloss = gloss.Trim();
        if (normalizedGloss.Length == 0)
            return 0;

        var glossLower = normalizedGloss.ToLowerInvariant();
        if (glossLower == keywordLower)
            return 500;

        if (ContainsWholeWord(glossLower, keywordLower))
            return 400;

        if (glossLower.StartsWith(keywordLower, StringComparison.Ordinal))
            return 300;

        return glossLower.Contains(keywordLower, StringComparison.Ordinal) ? 200 : 0;
    }

    private static bool ContainsWholeWord(string textLower, string keywordLower)
    {
        var index = textLower.IndexOf(keywordLower, StringComparison.Ordinal);

        while (index >= 0)
        {
            var beforeIsWord = index > 0 && IsAsciiWordChar(textLower[index - 1]);
            var end = index + keywordLower.Length;
            var afterIsWord = end < textLower.Length && IsAsciiWordChar(textLower[end]);

            if (!beforeIsWord && !afterIsWord)
                return true;

            index = textLower.IndexOf(keywordLower, index + 1, StringComparison.Ordinal);
        }

        return false;
    }

    private static bool IsAsciiWordChar(char ch)
    {
        return (ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9');
    }
}
