using Torisho.Application.DTOs.Flashcard;
using Torisho.Application.Interfaces.Flashcard;
using Torisho.Domain.Entities.DictionaryDomain;
using Torisho.Domain.Entities.FlashcardDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Application.Services.Flashcard;

public sealed class FlashcardStudyService : IFlashcardStudyService
{
    private readonly IUnitOfWork _unitOfWork;

    public FlashcardStudyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Guid> AddFromDictionaryAsync(Guid userId, Guid deckId, AddFromDictionaryRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var isOwnedByUser = await _unitOfWork.FlashcardDecks
            .IsOwnedByUserAsync(deckId, userId, ct);
        if (!isOwnedByUser)
            throw new UnauthorizedAccessException("Invalid deck ownership.");

        var entry = await _unitOfWork.DictionaryEntries.GetByIdAsync(request.DictionaryEntryId, ct);
        if (entry is null)
            throw new KeyNotFoundException("Dictionary entry not found.");

        var item = new FlashcardItem(
            deckId: deckId,
            front: entry.Keyword,
            back: ResolveBackText(entry),
            sourceType: "dictionary",
            dictionaryEntryId: request.DictionaryEntryId);

        await _unitOfWork.FlashcardItems.AddAsync(item, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return item.Id;
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

    private static string ResolveBackText(DictionaryEntry entry)
    {
        if (!string.IsNullOrWhiteSpace(entry.Definition?.GlossText))
            return entry.Definition.GlossText;

        if (!string.IsNullOrWhiteSpace(entry.MeaningsJson))
            return entry.MeaningsJson;

        return entry.Reading;
    }
}