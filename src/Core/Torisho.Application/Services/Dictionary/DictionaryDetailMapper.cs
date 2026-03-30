using System.Text.Json;
using Torisho.Application.DTOs.Dictionary;

namespace Torisho.Application.Services.Dictionary;

internal static class DictionaryDetailMapper
{
    internal static (string? Kanji, string? Kana, List<SenseDto> Senses) ParseRawJson(string? rawJson)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
            return (null, null, new List<SenseDto>());

        try
        {
            using var doc = JsonDocument.Parse(rawJson);
            var root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
                return (null, null, new List<SenseDto>());

            var (kanji, kana, _) = ExtractPrimaryForms(root);
            var senses = ExtractSenses(root);
            return (kanji, kana, senses);
        }
        catch (JsonException)
        {
            return (null, null, new List<SenseDto>());
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

    private static List<SenseDto> ExtractSenses(JsonElement wordObj)
    {
        if (!wordObj.TryGetProperty("sense", out var senseList) || senseList.ValueKind != JsonValueKind.Array)
            return new List<SenseDto>();

        var senses = new List<SenseDto>();

        foreach (var sense in senseList.EnumerateArray())
        {
            if (sense.ValueKind != JsonValueKind.Object)
                continue;

            var pos = new List<string>();
            if (sense.TryGetProperty("partOfSpeech", out var posList) && posList.ValueKind == JsonValueKind.Array)
            {
                foreach (var p in posList.EnumerateArray())
                {
                    if (p.ValueKind == JsonValueKind.String)
                    {
                        var s = p.GetString();
                        if (!string.IsNullOrWhiteSpace(s))
                            pos.Add(s);
                    }
                }
            }

            var glosses = new List<string>();
            if (sense.TryGetProperty("gloss", out var glossList) && glossList.ValueKind == JsonValueKind.Array)
            {
                foreach (var g in glossList.EnumerateArray())
                {
                    if (g.ValueKind == JsonValueKind.String)
                    {
                        var s = g.GetString();
                        if (!string.IsNullOrWhiteSpace(s))
                            glosses.Add(s);
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
                            glosses.Add(s);
                    }
                }
            }

            senses.Add(new SenseDto(pos, glosses));
        }

        return senses;
    }
}
