using System.Text.Json;
using Torisho.Application.DTOs.Flashcard;
using Torisho.Application.Interfaces.Flashcard;
using Torisho.Domain.Entities.DictionaryDomain;
using Torisho.Domain.Entities.FlashcardDomain;
using Torisho.Domain.Interfaces;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Application.Services.Flashcard;

public sealed class FlashcardStudyService : IFlashcardStudyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITatoeba _tatoeba;

    public FlashcardStudyService(IUnitOfWork unitOfWork, ITatoeba tatoeba)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _tatoeba = tatoeba ?? throw new ArgumentNullException(nameof(tatoeba));
    }

    public async Task<Guid> AddFromDictionaryAsync(Guid userId, Guid deckId, AddFromDictionaryRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.MaxTatoebaExamples < 0)
            throw new ArgumentException("MaxTatoebaExamples cannot be negative.", nameof(request.MaxTatoebaExamples));

        var isOwnedByUser = await _unitOfWork.FlashcardDecks
            .IsOwnedByUserAsync(deckId, userId, ct);
        if (!isOwnedByUser)
            throw new UnauthorizedAccessException("Invalid deck ownership.");

        var entry = await _unitOfWork.DictionaryEntries.GetByIdAsync(request.DictionaryEntryId, ct);
        if (entry is null)
            throw new KeyNotFoundException("Dictionary entry not found.");

        var front = ResolveFrontText(entry);
        var back = await ResolveBackTextAsync(entry, request.IncludeTatoebaExamples, request.MaxTatoebaExamples, ct);

        var item = new FlashcardItem(
            deckId: deckId,
            front: front,
            back: back,
            sourceType: "dictionary",
            dictionaryEntryId: request.DictionaryEntryId);

        await _unitOfWork.FlashcardItems.AddAsync(item, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return item.Id;
    }

    
    public async Task UpdateItemAsync(Guid userId, Guid deckId, Guid itemId, UpdateFlashcardItemRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var isOwnedByUser = await _unitOfWork.FlashcardDecks
            .IsOwnedByUserAsync(deckId, userId, ct);
        if (!isOwnedByUser)
            throw new UnauthorizedAccessException("Invalid deck ownership.");

        var item = await _unitOfWork.FlashcardItems.GetByIdAsync(itemId, ct);
        if (item is null || item.DeckId != deckId)
            throw new KeyNotFoundException("Flashcard item not found.");

        item.UpdateContent(request.Front, request.Back, request.Note);

        if (request.Position.HasValue)
            item.SetPosition(request.Position.Value);

        if (request.IsFavorite.HasValue)
            item.SetFavorite(request.IsFavorite.Value);

        await _unitOfWork.FlashcardItems.UpdateAsync(item, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteItemAsync(Guid userId, Guid deckId, Guid itemId, CancellationToken ct = default)
    {
        var isOwnedByUser = await _unitOfWork.FlashcardDecks
            .IsOwnedByUserAsync(deckId, userId, ct);
        if (!isOwnedByUser)
            throw new UnauthorizedAccessException("Invalid deck ownership.");

        var item = await _unitOfWork.FlashcardItems.GetByIdAsync(itemId, ct);
        if (item is null || item.DeckId != deckId)
            throw new KeyNotFoundException("Flashcard item not found.");

        await _unitOfWork.FlashcardItems.DeleteAsync(item, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<int> BulkImportAsync(Guid userId, Guid deckId, BulkImportRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var isOwnedByUser = await _unitOfWork.FlashcardDecks
            .IsOwnedByUserAsync(deckId, userId, ct);
        if (!isOwnedByUser)
            throw new UnauthorizedAccessException("Invalid deck ownership.");

        if (string.IsNullOrWhiteSpace(request.RawText))
            return 0;

        var cardSeparator = NormalizeSeparator(request.CardSeparator, "\n");
        var termDefinitionSeparator = NormalizeSeparator(request.TermDefinitionSeparator, "\t");
        var splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

        var rawCards = IsNewLineCardSeparator(cardSeparator)
            ? request.RawText.Split(new[] { "\r\n", "\n" }, splitOptions)
            : request.RawText.Split(cardSeparator, splitOptions);

        var itemsToInsert = new List<FlashcardItem>();

        foreach (var card in rawCards)
        {
            if (TryParseCard(card, termDefinitionSeparator, out var front, out var back))
            {
                var cleanedBack = CleanBackForQuizlet(back);

                var item = new FlashcardItem(
                    deckId: deckId,
                    front: front,
                    back: cleanedBack,
                    dictionaryEntryId: null,
                    sourceType: "bulk_import");

                itemsToInsert.Add(item);
            }
        }

        if (itemsToInsert.Count > 0)
        {
            await _unitOfWork.FlashcardItems.AddRangeAsync(itemsToInsert, ct);

            var deck = await _unitOfWork.FlashcardDecks.GetByIdAsync(deckId, ct);
            if (deck is not null)
                deck.MarkImported("manual_bulk", $"Imported {itemsToInsert.Count} items");

            await _unitOfWork.SaveChangesAsync(ct);
        }

        return itemsToInsert.Count;
    }

    private static bool IsNewLineCardSeparator(string cardSeparator)
        => cardSeparator is "\n" or "\r\n";

    private static string NormalizeSeparator(string? separator, string fallback)
    {
        if (string.IsNullOrEmpty(separator))
            return fallback;

        var normalized = separator
            .Replace("\\r\\n", "\r\n", StringComparison.Ordinal)
            .Replace("\\n", "\n", StringComparison.Ordinal)
            .Replace("\\t", "\t", StringComparison.Ordinal)
            .Replace("\\r", "\r", StringComparison.Ordinal);

        return string.IsNullOrEmpty(normalized) ? fallback : normalized;
    }

    private static bool TryParseCard(string card, string separator, out string front, out string back)
    {
        front = string.Empty;
        back = string.Empty;

        if (string.IsNullOrWhiteSpace(card))
            return false;

        var lines = card.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (lines.Length == 0)
            return false;

        var firstLine = lines[0];
        var separatorIndex = firstLine.IndexOf(separator, StringComparison.Ordinal);

        // Preferred format: only the first line contains "Front<TAB>..." and following lines are extra back content.
        if (separatorIndex >= 0)
        {
            front = firstLine[..separatorIndex].Trim();

            var backLines = new List<string>();

            var firstBackSegment = firstLine[(separatorIndex + separator.Length)..].Trim();
            if (!string.IsNullOrWhiteSpace(firstBackSegment))
                backLines.Add(firstBackSegment);

            for (var i = 1; i < lines.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(lines[i]))
                    backLines.Add(lines[i]);
            }

            back = string.Join("\n", backLines).Trim();
            return !string.IsNullOrWhiteSpace(front) && !string.IsNullOrWhiteSpace(back);
        }

        // Fallback: handle old single-card text where separator appears across the full block.
        var rawIndex = card.IndexOf(separator, StringComparison.Ordinal);
        if (rawIndex < 0)
            return false;

        front = card[..rawIndex].Trim();
        back = card[(rawIndex + separator.Length)..].Trim();

        return !string.IsNullOrWhiteSpace(front) && !string.IsNullOrWhiteSpace(back);
    }

    private static string CleanBackForQuizlet(string back)
    {
        if (string.IsNullOrWhiteSpace(back))
            return string.Empty;

        var normalized = back
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal);

        var lines = normalized.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return string.Join("<br>", lines);
    }

    private static string ResolveFrontText(DictionaryEntry entry)
    {
        var keyword = entry.Keyword?.Trim();
        if (ContainsKanji(keyword))
            return keyword!;

        var kanjiFromRaw = TryExtractKanjiFromRawJson(entry.RawJson);
        if (!string.IsNullOrWhiteSpace(kanjiFromRaw))
            return kanjiFromRaw;

        if (!string.IsNullOrWhiteSpace(keyword))
            return keyword;

        if (!string.IsNullOrWhiteSpace(entry.Reading))
            return entry.Reading.Trim();

        return "(empty)";
    }

    private async Task<string> ResolveBackTextAsync(
        DictionaryEntry entry,
        bool includeTatoebaExamples,
        int maxTatoebaExamples,
        CancellationToken ct)
    {
        var lines = new List<string>();

        var reading = entry.Reading?.Trim();
        if (!string.IsNullOrWhiteSpace(reading))
            lines.Add(reading);

        var meaning = ResolveMeaningText(entry);
        if (!string.IsNullOrWhiteSpace(meaning))
            lines.Add(meaning);

        if (includeTatoebaExamples && maxTatoebaExamples > 0)
        {
            var examples = await _tatoeba.GetExamplesAsync(entry.Keyword, ct);

            if (examples.Count == 0 && !string.IsNullOrWhiteSpace(entry.Reading))
                examples = await _tatoeba.GetExamplesAsync(entry.Reading, ct);

            foreach (var example in examples.Take(maxTatoebaExamples))
            {
                if (string.IsNullOrWhiteSpace(example.Japanese) || string.IsNullOrWhiteSpace(example.English))
                    continue;

                lines.Add($"{example.Japanese} - {example.English}");
            }
        }

        if (lines.Count == 0)
            lines.Add(entry.Reading ?? string.Empty);

        // Store multiline content in DB as <br> so current UI can render line breaks consistently.
        return string.Join("<br>", lines.Where(line => !string.IsNullOrWhiteSpace(line)));
    }

    private static string ResolveMeaningText(DictionaryEntry entry)
    {
        if (!string.IsNullOrWhiteSpace(entry.Definition?.GlossText))
            return entry.Definition.GlossText.Trim();

        if (!string.IsNullOrWhiteSpace(entry.MeaningsJson))
        {
            try
            {
                using var doc = JsonDocument.Parse(entry.MeaningsJson);
                var texts = new List<string>();
                CollectMeaningTexts(doc.RootElement, texts);

                if (texts.Count > 0)
                    return string.Join("; ", texts.Distinct());
            }
            catch (JsonException)
            {
                // Fall through to raw text.
            }

            return entry.MeaningsJson.Trim();
        }

        return string.Empty;
    }

    private static void CollectMeaningTexts(JsonElement element, List<string> collector)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
            {
                var value = element.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                    collector.Add(value.Trim());
                break;
            }
            case JsonValueKind.Array:
                foreach (var child in element.EnumerateArray())
                    CollectMeaningTexts(child, collector);
                break;
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    if (property.Value.ValueKind is JsonValueKind.String or JsonValueKind.Array)
                        CollectMeaningTexts(property.Value, collector);
                }
                break;
        }
    }

    private static bool ContainsKanji(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        return text.Any(ch =>
            (ch >= '\u4E00' && ch <= '\u9FFF') ||
            (ch >= '\u3400' && ch <= '\u4DBF') ||
            ch == '々');
    }

    private static string? TryExtractKanjiFromRawJson(string? rawJson)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
            return null;

        try
        {
            using var doc = JsonDocument.Parse(rawJson);
            var root = doc.RootElement;

            if (!root.TryGetProperty("kanji", out var kanjiArray) || kanjiArray.ValueKind != JsonValueKind.Array)
                return null;

            foreach (var item in kanjiArray.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object)
                    continue;

                if (!item.TryGetProperty("text", out var textEl) || textEl.ValueKind != JsonValueKind.String)
                    continue;

                var value = textEl.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                    return value.Trim();
            }

            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}