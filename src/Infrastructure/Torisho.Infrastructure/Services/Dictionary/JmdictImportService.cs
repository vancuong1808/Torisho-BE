using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Application.Services.Dictionary;
using Torisho.Domain.Entities.DictionaryDomain;
using Torisho.Domain.Enums;

namespace Torisho.Infrastructure.Services.Dictionary;

public sealed class JmdictImportService : IJmdictImportService
{
    private readonly IDataContext _context;

    public JmdictImportService(IDataContext context)
    {
        _context = context;
    }

    public async Task<JmdictImportResult> ImportAsync(Stream utf8JsonStream, CancellationToken ct = default)
    {
        if (utf8JsonStream is null)
            throw new ArgumentNullException(nameof(utf8JsonStream));

        var created = 0;
        var updated = 0;
        var skipped = 0;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var batchCount = 0;
        const int batchSize = 500;

        await foreach (var element in JsonSerializer.DeserializeAsyncEnumerable<JsonElement>(utf8JsonStream, jsonOptions, ct))
        {
            if (ct.IsCancellationRequested)
                break;

            if (element.ValueKind != JsonValueKind.Object)
            {
                skipped++;
                continue;
            }

            var rawJson = element.GetRawText();

            var sourceId = TryGetString(element, "id") ?? TryGetString(element, "ent_seq");

            var kanjiForms = ExtractForms(element, "kanji", "text");
            var readingForms = ExtractForms(element, "kana", "text");

            var primaryHeadword = kanjiForms.FirstOrDefault() ?? readingForms.FirstOrDefault();
            var primaryReading = readingForms.FirstOrDefault() ?? primaryHeadword;

            if (string.IsNullOrWhiteSpace(primaryHeadword) || string.IsNullOrWhiteSpace(primaryReading))
            {
                skipped++;
                continue;
            }

            var isCommon = ExtractIsCommon(element);
            var glossText = ExtractEnglishGlossText(element);

            DictionaryEntry? entry = null;
            if (!string.IsNullOrWhiteSpace(sourceId))
            {
                entry = await _context.Set<DictionaryEntry>()
                    .Include(e => e.KanjiForms)
                    .Include(e => e.ReadingForms)
                    .Include(e => e.Definition)
                    .FirstOrDefaultAsync(e => e.SourceId == sourceId, ct);
            }

            if (entry is null)
            {
                entry = new DictionaryEntry(
                    keyword: primaryHeadword,
                    reading: primaryReading,
                    jlpt: JLPTLevel.N5,
                    rawJson: rawJson,
                    sourceId: sourceId,
                    isCommon: isCommon);

                entry.ReplaceKanjiForms(kanjiForms.Select(k => (Text: k, IsCommon: isCommon)));
                entry.ReplaceReadingForms(readingForms);

                if (!string.IsNullOrWhiteSpace(glossText))
                    entry.SetGlossText(glossText);

                await _context.Set<DictionaryEntry>().AddAsync(entry, ct);
                created++;
            }
            else
            {
                entry.UpdatePrimaryForms(primaryHeadword, primaryReading);
                entry.SetRawJson(rawJson);
                entry.SetIsCommon(isCommon);

                entry.ReplaceKanjiForms(kanjiForms.Select(k => (Text: k, IsCommon: isCommon)));
                entry.ReplaceReadingForms(readingForms);

                if (!string.IsNullOrWhiteSpace(glossText))
                    entry.SetGlossText(glossText);

                updated++;
            }

            batchCount++;
            if (batchCount >= batchSize)
            {
                await _context.SaveChangesAsync(ct);
                batchCount = 0;
            }
        }

        if (batchCount > 0)
            await _context.SaveChangesAsync(ct);

        return new JmdictImportResult(created, updated, skipped);
    }

    private static string? TryGetString(JsonElement obj, string propertyName)
    {
        if (!obj.TryGetProperty(propertyName, out var prop))
            return null;

        return prop.ValueKind switch
        {
            JsonValueKind.String => prop.GetString(),
            JsonValueKind.Number => prop.TryGetInt64(out var n) ? n.ToString() : prop.ToString(),
            _ => prop.ToString()
        };
    }

    private static List<string> ExtractForms(JsonElement obj, string listProperty, string textProperty)
    {
        var result = new List<string>();

        if (!obj.TryGetProperty(listProperty, out var list) || list.ValueKind != JsonValueKind.Array)
            return result;

        foreach (var item in list.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object)
                continue;

            if (item.TryGetProperty(textProperty, out var text) && text.ValueKind == JsonValueKind.String)
            {
                var value = text.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                    result.Add(value);
            }
        }

        return result.Distinct(StringComparer.Ordinal).ToList();
    }

    private static bool ExtractIsCommon(JsonElement obj)
    {
        static bool IsCommonFlag(JsonElement item)
        {
            if (item.ValueKind != JsonValueKind.Object)
                return false;

            if (!item.TryGetProperty("common", out var common))
                return false;

            return common.ValueKind switch
            {
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Number => common.TryGetInt32(out var n) && n != 0,
                JsonValueKind.String => bool.TryParse(common.GetString(), out var b) && b,
                _ => false
            };
        }

        var kanjiCommon = obj.TryGetProperty("kanji", out var kanji) && kanji.ValueKind == JsonValueKind.Array && kanji.EnumerateArray().Any(IsCommonFlag);
        var kanaCommon = obj.TryGetProperty("kana", out var kana) && kana.ValueKind == JsonValueKind.Array && kana.EnumerateArray().Any(IsCommonFlag);
        return kanjiCommon || kanaCommon;
    }

    private static string ExtractEnglishGlossText(JsonElement obj)
    {
        if (!obj.TryGetProperty("sense", out var sense) || sense.ValueKind != JsonValueKind.Array)
            return string.Empty;

        var glosses = new List<string>();

        foreach (var senseItem in sense.EnumerateArray())
        {
            if (senseItem.ValueKind != JsonValueKind.Object)
                continue;

            if (!senseItem.TryGetProperty("gloss", out var gloss) || gloss.ValueKind != JsonValueKind.Array)
                continue;

            foreach (var g in gloss.EnumerateArray())
            {
                switch (g.ValueKind)
                {
                    case JsonValueKind.String:
                        if (!string.IsNullOrWhiteSpace(g.GetString()))
                            glosses.Add(g.GetString()!);
                        break;

                    case JsonValueKind.Object:
                        // JMdict JSON often uses: {"lang":"eng", "text":"..."}
                        var lang = g.TryGetProperty("lang", out var langEl) && langEl.ValueKind == JsonValueKind.String
                            ? langEl.GetString()
                            : null;

                        if (!string.IsNullOrWhiteSpace(lang) && !string.Equals(lang, "eng", StringComparison.OrdinalIgnoreCase))
                            break;

                        if (g.TryGetProperty("text", out var textEl) && textEl.ValueKind == JsonValueKind.String)
                        {
                            var txt = textEl.GetString();
                            if (!string.IsNullOrWhiteSpace(txt))
                                glosses.Add(txt);
                        }
                        break;
                }
            }
        }

        return string.Join("; ", glosses.Distinct(StringComparer.OrdinalIgnoreCase));
    }
}
