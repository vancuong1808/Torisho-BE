using System.Text.Json;
using Torisho.Application.DTOs.Dictionary;

namespace Torisho.Infrastructure.Services.Dictionary;

internal static class DictionaryRawJsonMapper
{
    internal static WordSchemaDto? TryParseWord(string? rawJson)
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
            var primaryMeaning = ExtractPrimaryMeaning(root);

            // Id will be filled from DB row.
            return new WordSchemaDto(
                Id: Guid.Empty,
                Kanji: kanji,
                Kana: kana ?? string.Empty,
                IsCommon: isCommon,
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

    private static string ExtractPrimaryMeaning(JsonElement wordObj)
    {
        if (!wordObj.TryGetProperty("sense", out var senseList) || senseList.ValueKind != JsonValueKind.Array)
            return string.Empty;

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
                        return s;
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
                        return s;
                }
            }
        }

        return string.Empty;
    }
}
