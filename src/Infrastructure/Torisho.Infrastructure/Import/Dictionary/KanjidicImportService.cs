using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Application.Interfaces.Dictionary;
using Torisho.Domain.Entities.DictionaryDomain;

namespace Torisho.Infrastructure.Import.Dictionary;

public sealed class KanjidicImportService : IKanjidicImportService
{
    private readonly IDataContext _context;

    public KanjidicImportService(IDataContext context)
    {
        _context = context;
    }

    public async Task<KanjidicImportResult> ImportAsync(
        string kanjidicDirectoryPath,
        bool rebuildEntryLinks = true,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(kanjidicDirectoryPath))
            throw new ArgumentException("KANJIDIC directory path is required", nameof(kanjidicDirectoryPath));

        var fullDirectoryPath = Path.GetFullPath(kanjidicDirectoryPath);
        if (!Directory.Exists(fullDirectoryPath))
            throw new DirectoryNotFoundException($"KANJIDIC directory not found: {fullDirectoryPath}");

        var bankFiles = Directory
            .EnumerateFiles(fullDirectoryPath, "kanji_bank_*.json", SearchOption.TopDirectoryOnly)
            .OrderBy(GetBankFileOrder)
            .ThenBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (bankFiles.Count == 0)
            throw new FileNotFoundException($"No kanji_bank_*.json files found in '{fullDirectoryPath}'.");

        var created = 0;
        var updated = 0;
        var skipped = 0;
        var linked = 0;
        var linkSkipped = 0;
        var processedFiles = 0;

        const int batchSize = 1000;
        var batch = new List<ParsedKanji>(batchSize);

        var dbContext = _context as DbContext;
        var previousAutoDetect = dbContext?.ChangeTracker.AutoDetectChangesEnabled;
        if (dbContext is not null)
            dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

        try
        {
            foreach (var bankFile in bankFiles)
            {
                ct.ThrowIfCancellationRequested();

                await using var stream = new FileStream(
                    bankFile,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 1024 * 64,
                    options: FileOptions.SequentialScan);

                using var document = await JsonDocument.ParseAsync(
                    stream,
                    new JsonDocumentOptions
                    {
                        AllowTrailingCommas = true,
                        CommentHandling = JsonCommentHandling.Skip
                    },
                    ct);

                if (document.RootElement.ValueKind != JsonValueKind.Array)
                    throw new JsonException($"KANJIDIC bank file '{Path.GetFileName(bankFile)}' must be a top-level JSON array.");

                foreach (var element in document.RootElement.EnumerateArray())
                {
                    ct.ThrowIfCancellationRequested();

                    var parsed = ParseKanji(element);
                    if (parsed is null)
                    {
                        skipped++;
                        continue;
                    }

                    batch.Add(parsed);
                    if (batch.Count >= batchSize)
                    {
                        var (c, u, s) = await UpsertKanjiBatchAsync(batch, ct);
                        created += c;
                        updated += u;
                        skipped += s;
                        batch.Clear();

                        dbContext?.ChangeTracker.Clear();
                    }
                }

                processedFiles++;
            }

            if (batch.Count > 0)
            {
                var (c, u, s) = await UpsertKanjiBatchAsync(batch, ct);
                created += c;
                updated += u;
                skipped += s;
                batch.Clear();

                dbContext?.ChangeTracker.Clear();
            }

            if (rebuildEntryLinks)
            {
                var (createdLinks, skippedLinks) = await RebuildEntryKanjiLinksAsync(ct);
                linked += createdLinks;
                linkSkipped += skippedLinks;
            }
        }
        finally
        {
            if (dbContext is not null && previousAutoDetect.HasValue)
                dbContext.ChangeTracker.AutoDetectChangesEnabled = previousAutoDetect.Value;
        }

        return new KanjidicImportResult(
            ProcessedFiles: processedFiles,
            Created: created,
            Updated: updated,
            Skipped: skipped,
            Linked: linked,
            LinkSkipped: linkSkipped);
    }

    private async Task<(int Created, int Updated, int Skipped)> UpsertKanjiBatchAsync(
        List<ParsedKanji> batch,
        CancellationToken ct)
    {
        var created = 0;
        var updated = 0;
        var skipped = 0;

        var deduped = batch
            .GroupBy(x => x.Character, StringComparer.Ordinal)
            .Select(g => g.Last())
            .ToList();

        skipped += batch.Count - deduped.Count;

        var characters = deduped.Select(x => x.Character).ToList();
        var existing = await _context.Set<Kanji>()
            .Where(x => characters.Contains(x.Character))
            .ToDictionaryAsync(x => x.Character, ct);

        foreach (var item in deduped)
        {
            try
            {
                if (existing.TryGetValue(item.Character, out var kanji))
                {
                    kanji.UpdateMetadata(
                        onyomi: item.Onyomi,
                        kunyomi: item.Kunyomi,
                        type: item.Type,
                        meaningsJson: item.MeaningsJson,
                        jlptLevel: item.JlptLevel,
                        grade: item.Grade,
                        strokeCount: item.StrokeCount,
                        frequency: item.Frequency,
                        unicodeHex: item.UnicodeHex);

                    updated++;
                }
                else
                {
                    var entity = new Kanji(
                        character: item.Character,
                        onyomi: item.Onyomi,
                        kunyomi: item.Kunyomi,
                        type: item.Type,
                        meaningsJson: item.MeaningsJson,
                        jlptLevel: item.JlptLevel,
                        grade: item.Grade,
                        strokeCount: item.StrokeCount,
                        frequency: item.Frequency,
                        unicodeHex: item.UnicodeHex);

                    await _context.Set<Kanji>().AddAsync(entity, ct);
                    created++;
                }
            }
            catch (ArgumentException)
            {
                skipped++;
            }
        }

        await _context.SaveChangesAsync(ct);
        return (created, updated, skipped);
    }

    private async Task<(int Linked, int Skipped)> RebuildEntryKanjiLinksAsync(CancellationToken ct)
    {
        await _context.Set<DictionaryEntryKanji>().ExecuteDeleteAsync(ct);

        var kanjiLookup = await _context.Set<Kanji>()
            .AsNoTracking()
            .Select(x => new { x.Id, x.Character })
            .ToDictionaryAsync(x => x.Character, x => x.Id, ct);

        var forms = await _context.Set<DictionaryEntryKanjiForm>()
            .AsNoTracking()
            .ToListAsync(ct);

        const int linkBatchSize = 3000;
        var links = new List<DictionaryEntryKanji>(linkBatchSize);
        var seen = new HashSet<(Guid EntryId, Guid KanjiId, int Position)>();

        var linked = 0;
        var skipped = 0;

        foreach (var form in forms)
        {
            var position = 0;
            foreach (var rune in form.KanjiText.EnumerateRunes())
            {
                var character = rune.ToString();
                if (!kanjiLookup.TryGetValue(character, out var kanjiId))
                {
                    skipped++;
                    position++;
                    continue;
                }

                var key = (form.DictionaryEntryId, kanjiId, position);
                if (!seen.Add(key))
                {
                    skipped++;
                    position++;
                    continue;
                }

                links.Add(new DictionaryEntryKanji(form.DictionaryEntryId, kanjiId, position));
                position++;

                if (links.Count >= linkBatchSize)
                    linked += await SaveLinkBatchAsync(links, ct);
            }
        }

        if (links.Count > 0)
            linked += await SaveLinkBatchAsync(links, ct);

        return (linked, skipped);
    }

    private async Task<int> SaveLinkBatchAsync(List<DictionaryEntryKanji> batch, CancellationToken ct)
    {
        var count = batch.Count;

        await _context.Set<DictionaryEntryKanji>().AddRangeAsync(batch, ct);
        await _context.SaveChangesAsync(ct);

        batch.Clear();

        if (_context is DbContext dbContext)
            dbContext.ChangeTracker.Clear();

        return count;
    }

    private sealed record ParsedKanji(
        string Character,
        string Onyomi,
        string Kunyomi,
        string Type,
        string MeaningsJson,
        int? JlptLevel,
        int? Grade,
        int StrokeCount,
        int? Frequency,
        string? UnicodeHex);

    private static ParsedKanji? ParseKanji(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Array)
            return null;

        var parts = element.EnumerateArray().ToList();
        if (parts.Count < 6)
            return null;

        var character = ReadText(parts[0]);
        if (string.IsNullOrWhiteSpace(character))
            return null;

        var onyomi = ReadText(parts[1]);
        var kunyomi = ReadText(parts[2]);
        var type = ReadText(parts[3]).ToLowerInvariant();

        var meanings = ExtractMeanings(parts[4]);
        var meaningsJson = JsonSerializer.Serialize(meanings);

        if (parts[5].ValueKind != JsonValueKind.Object)
            return null;

        var metadata = parts[5];
        var strokeCount = TryReadInt(metadata, "strokes");
        if (!strokeCount.HasValue)
            return null;

        var jlptLevel = TryReadInt(metadata, "jlpt");
        var grade = TryReadInt(metadata, "grade");
        var frequency = TryReadInt(metadata, "freq");
        var unicodeHex = ReadMetaText(metadata, "ucs")?.ToLowerInvariant();

        return new ParsedKanji(
            Character: character,
            Onyomi: onyomi,
            Kunyomi: kunyomi,
            Type: type,
            MeaningsJson: meaningsJson,
            JlptLevel: jlptLevel,
            Grade: grade,
            StrokeCount: strokeCount.Value,
            Frequency: frequency,
            UnicodeHex: unicodeHex);
    }

    private static List<string> ExtractMeanings(JsonElement meaningsElement)
    {
        var result = new List<string>();

        if (meaningsElement.ValueKind != JsonValueKind.Array)
            return result;

        foreach (var item in meaningsElement.EnumerateArray())
        {
            var text = ReadText(item);
            if (!string.IsNullOrWhiteSpace(text))
                result.Add(text);
        }

        return result;
    }

    private static string ReadText(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString()?.Trim() ?? string.Empty,
            JsonValueKind.Null => string.Empty,
            JsonValueKind.Undefined => string.Empty,
            _ => element.ToString().Trim()
        };
    }

    private static string? ReadMetaText(JsonElement metadata, string key)
    {
        if (!metadata.TryGetProperty(key, out var value))
            return null;

        var text = ReadText(value);
        return string.IsNullOrWhiteSpace(text) ? null : text;
    }

    private static int? TryReadInt(JsonElement metadata, string key)
    {
        if (!metadata.TryGetProperty(key, out var value))
            return null;

        return value.ValueKind switch
        {
            JsonValueKind.Number when value.TryGetInt32(out var n) => n,
            JsonValueKind.String when int.TryParse(value.GetString(), out var n) => n,
            _ => null
        };
    }

    private static int GetBankFileOrder(string filePath)
    {
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        if (fileName.StartsWith("kanji_bank_", StringComparison.OrdinalIgnoreCase)
            && int.TryParse(fileName["kanji_bank_".Length..], out var order))
        {
            return order;
        }

        return int.MaxValue;
    }
}
