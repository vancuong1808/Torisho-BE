using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Application.Interfaces.Dictionary;
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

        // The JMdict JSON file is shaped like:
        // { "version": "...", "words": [ { ... }, ... ] }
        // (i.e., NOT a JSON array at the root)
        using var document = await JsonDocument.ParseAsync(
            utf8JsonStream,
            new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            },
            ct);

        if (!document.RootElement.TryGetProperty("words", out var words) || words.ValueKind != JsonValueKind.Array)
            throw new JsonException("JMdict JSON must contain a top-level 'words' array.");

        const int batchSize = 1000;
        var batch = new List<ParsedWord>(batchSize);

        var dbContext = _context as DbContext;
        var previousAutoDetect = dbContext?.ChangeTracker.AutoDetectChangesEnabled;
        if (dbContext is not null)
            dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

        try
        {
            foreach (var element in words.EnumerateArray())
            {
                if (ct.IsCancellationRequested)
                    break;

                if (element.ValueKind != JsonValueKind.Object)
                {
                    skipped++;
                    continue;
                }

                var parsed = ParseWord(element);
                if (parsed is null)
                {
                    skipped++;
                    continue;
                }

                batch.Add(parsed);
                if (batch.Count >= batchSize)
                {
                    var (c, u, s) = await UpsertBatchAsync(batch, ct);
                    created += c;
                    updated += u;
                    skipped += s;
                    batch.Clear();

                    dbContext?.ChangeTracker.Clear();
                }
            }

            if (batch.Count > 0)
            {
                var (c, u, s) = await UpsertBatchAsync(batch, ct);
                created += c;
                updated += u;
                skipped += s;
            }
        }
        finally
        {
            if (dbContext is not null && previousAutoDetect.HasValue)
                dbContext.ChangeTracker.AutoDetectChangesEnabled = previousAutoDetect.Value;
        }

        return new JmdictImportResult(created, updated, skipped);
    }

    private sealed record ParsedWord(
        string? SourceId,
        string PrimaryHeadword,
        string PrimaryReading,
        bool IsCommon,
        string RawJson,
        List<(string Text, bool IsCommon)> KanjiForms,
        List<string> ReadingForms,
        string GlossText);

    private static string NormalizeFormText(string value)
        => value.Trim().Normalize(NormalizationForm.FormKC);

    private static string NormalizeKey(string value)
        => FoldKana(RemoveDiacritics(NormalizeFormText(value))).ToUpperInvariant();

    private static string RemoveDiacritics(string value)
    {
        var decomposed = value.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(decomposed.Length);

        foreach (var ch in decomposed)
        {
            var uc = char.GetUnicodeCategory(ch);
            if (uc is System.Globalization.UnicodeCategory.NonSpacingMark
                or System.Globalization.UnicodeCategory.SpacingCombiningMark
                or System.Globalization.UnicodeCategory.EnclosingMark)
                continue;

            sb.Append(ch);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    private static string FoldKana(string value)
    {
        // Many MySQL collations are kana-insensitive. Fold Katakana to Hiragana for dedupe keys.
        // Katakana range: U+30A1..U+30F6 maps to Hiragana U+3041..U+3096 by subtracting 0x60.
        var sb = new StringBuilder(value.Length);
        foreach (var ch in value)
        {
            if (ch is >= '\u30A1' and <= '\u30F6')
                sb.Append((char)(ch - 0x60));
            else
                sb.Append(ch);
        }
        return sb.ToString();
    }

    private static ParsedWord? ParseWord(JsonElement element)
    {
        var rawJson = element.GetRawText();

        // Prefer ent_seq (JMdict stable id) over any exporter-specific "id" field.
        var sourceId = TryGetString(element, "ent_seq") ?? TryGetString(element, "id");

        var kanjiForms = ExtractKanjiForms(element);
        var readingForms = ExtractReadingForms(element);

        var primaryHeadword = kanjiForms.FirstOrDefault().Text ?? readingForms.FirstOrDefault();
        var primaryReading = readingForms.FirstOrDefault() ?? primaryHeadword;

        if (string.IsNullOrWhiteSpace(primaryHeadword) || string.IsNullOrWhiteSpace(primaryReading))
            return null;

        var isCommon = ExtractIsCommon(element);
        var glossText = ExtractEnglishGlossText(element);

        return new ParsedWord(
            SourceId: sourceId,
            PrimaryHeadword: primaryHeadword,
            PrimaryReading: primaryReading,
            IsCommon: isCommon,
            RawJson: rawJson,
            KanjiForms: kanjiForms,
            ReadingForms: readingForms,
            GlossText: glossText);
    }

    private async Task<(int Created, int Updated, int Skipped)> UpsertBatchAsync(List<ParsedWord> batch, CancellationToken ct)
    {
        var created = 0;
        var updated = 0;
        var skipped = 0;

        // Ensure we don't process the same SourceId multiple times in a single batch.
        // (If it happens, it can cause duplicate dependent inserts before SaveChanges.)
        var uniqueBySourceId = batch
            .Where(x => !string.IsNullOrWhiteSpace(x.SourceId))
            .GroupBy(x => x.SourceId!, StringComparer.Ordinal)
            .Select(g => g.Last())
            .ToList();

        // Anything without SourceId is not safely idempotent; skip to avoid duplicates on re-runs.
        skipped += batch.Count - uniqueBySourceId.Count;
        batch = uniqueBySourceId;

        var sourceIds = batch
            .Select(x => x.SourceId!)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        Dictionary<string, DictionaryEntry> existingBySourceId = new(StringComparer.Ordinal);
        if (sourceIds.Count > 0)
        {
            // Load only entries (no Includes). We'll delete & re-insert dependents in bulk for updates.
            var existing = await _context.Set<DictionaryEntry>()
                .Where(e => e.SourceId != null && sourceIds.Contains(e.SourceId))
                .ToListAsync(ct);

            foreach (var e in existing)
            {
                if (!string.IsNullOrWhiteSpace(e.SourceId))
                    existingBySourceId[e.SourceId!] = e;
            }

            // Pre-delete dependent rows for entries that will be updated.
            var existingEntryIds = existing.Select(e => e.Id).ToList();
            if (existingEntryIds.Count > 0)
            {
                await _context.Set<DictionaryEntryKanjiForm>()
                    .Where(x => existingEntryIds.Contains(x.DictionaryEntryId))
                    .ExecuteDeleteAsync(ct);

                await _context.Set<DictionaryEntryReadingForm>()
                    .Where(x => existingEntryIds.Contains(x.DictionaryEntryId))
                    .ExecuteDeleteAsync(ct);

                await _context.Set<DictionaryEntryDefinition>()
                    .Where(x => existingEntryIds.Contains(x.DictionaryEntryId))
                    .ExecuteDeleteAsync(ct);
            }
        }

        foreach (var item in batch)
        {
            DictionaryEntry? entry = null;
            existingBySourceId.TryGetValue(item.SourceId!, out entry);

            // Final safety: dedupe again using a key that matches typical MySQL collations
            // (case-insensitive, width-insensitive after FormKC, and trimmed).
            var kanjiForms = item.KanjiForms
                .Where(x => !string.IsNullOrWhiteSpace(x.Text))
                .Select(x => (Text: NormalizeFormText(x.Text), x.IsCommon))
                .Where(x => !string.IsNullOrWhiteSpace(x.Text))
                .GroupBy(x => NormalizeKey(x.Text), StringComparer.Ordinal)
                .Select(g => (Text: g.First().Text, IsCommon: g.Any(x => x.IsCommon)))
                .ToList();

            var readingForms = item.ReadingForms
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(NormalizeFormText)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .GroupBy(NormalizeKey, StringComparer.Ordinal)
                .Select(g => g.First())
                .ToList();

            if (entry is null)
            {
                entry = new DictionaryEntry(
                    keyword: item.PrimaryHeadword,
                    reading: item.PrimaryReading,
                    jlpt: JLPTLevel.N5,
                    rawJson: item.RawJson,
                    sourceId: item.SourceId,
                    isCommon: item.IsCommon);

                entry.ReplaceKanjiForms(kanjiForms);
                entry.ReplaceReadingForms(readingForms);

                if (!string.IsNullOrWhiteSpace(item.GlossText))
                    entry.SetGlossText(item.GlossText);

                await _context.Set<DictionaryEntry>().AddAsync(entry, ct);
                created++;
            }
            else
            {
                entry.UpdatePrimaryForms(item.PrimaryHeadword, item.PrimaryReading);
                entry.SetRawJson(item.RawJson);
                entry.SetIsCommon(item.IsCommon);

                entry.ReplaceKanjiForms(kanjiForms);
                entry.ReplaceReadingForms(readingForms);

                if (!string.IsNullOrWhiteSpace(item.GlossText))
                    entry.SetGlossText(item.GlossText);

                updated++;
            }
        }

        if (_context is DbContext dbContext)
            dbContext.ChangeTracker.DetectChanges();

        // Hard-stop any remaining duplicates that may still arise due to database collation rules
        // (width/kana/diacritics/case-insensitivity). Detach duplicates so SaveChanges never fails.
        if (_context is DbContext db)
        {
            var kanjiAdds = db.ChangeTracker.Entries<DictionaryEntryKanjiForm>()
                .Where(e => e.State == EntityState.Added)
                .ToList();

            foreach (var group in kanjiAdds.GroupBy(e => (e.Entity.DictionaryEntryId, Key: NormalizeKey(e.Entity.KanjiText))))
            {
                foreach (var dup in group.Skip(1))
                    dup.State = EntityState.Detached;
            }

            var readingAdds = db.ChangeTracker.Entries<DictionaryEntryReadingForm>()
                .Where(e => e.State == EntityState.Added)
                .ToList();

            foreach (var group in readingAdds.GroupBy(e => (e.Entity.DictionaryEntryId, Key: NormalizeKey(e.Entity.ReadingText))))
            {
                foreach (var dup in group.Skip(1))
                    dup.State = EntityState.Detached;
            }
        }

        await _context.SaveChangesAsync(ct);

        return (created, updated, skipped);
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

    private static List<(string Text, bool IsCommon)> ExtractKanjiForms(JsonElement obj)
    {
        var result = new List<(string Text, bool IsCommon)>();

        if (!obj.TryGetProperty("kanji", out var list) || list.ValueKind != JsonValueKind.Array)
            return result;

        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var item in list.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object)
                continue;

            if (!item.TryGetProperty("text", out var textEl) || textEl.ValueKind != JsonValueKind.String)
                continue;

            var text = textEl.GetString();
            if (string.IsNullOrWhiteSpace(text))
                continue;

            var normalized = NormalizeFormText(text);
            var key = NormalizeKey(normalized);
            if (string.IsNullOrWhiteSpace(normalized) || !seen.Add(key))
                continue;

            var isCommon = item.TryGetProperty("common", out var commonEl) && commonEl.ValueKind == JsonValueKind.True;
            result.Add((normalized, isCommon));
        }

        return result;
    }

    private static List<string> ExtractReadingForms(JsonElement obj)
    {
        var result = new List<string>();

        if (!obj.TryGetProperty("kana", out var list) || list.ValueKind != JsonValueKind.Array)
            return result;

        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var item in list.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object)
                continue;

            if (!item.TryGetProperty("text", out var textEl) || textEl.ValueKind != JsonValueKind.String)
                continue;

            var text = textEl.GetString();
            if (string.IsNullOrWhiteSpace(text))
                continue;

            var normalized = NormalizeFormText(text);
            var key = NormalizeKey(normalized);
            if (string.IsNullOrWhiteSpace(normalized) || !seen.Add(key))
                continue;

            result.Add(normalized);
        }

        return result;
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
