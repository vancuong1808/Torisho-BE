using Microsoft.EntityFrameworkCore;
using Torisho.Application.DTOs.Flashcard;
using Torisho.Application.Interfaces.Flashcard;
using Torisho.Domain.Entities.FlashcardDomain;

namespace Torisho.Application.Services.Flashcard;

public sealed class FlashcardQueryService : IFlashcardQueryService
{
    private readonly IDataContext _context;

    public FlashcardQueryService(IDataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FlashcardDeckSummaryDto>> GetUserDecksAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        return await _context.Set<FlashcardDeck>()
            .AsNoTracking()
            .Where(d => d.UserId == userId && !d.IsArchived)
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => new FlashcardDeckSummaryDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                FolderId = d.FolderId,
                FolderName = d.Folder != null ? d.Folder.Name : null,
                TotalItems = d.Items.Count,
                CreatedAt = d.CreatedAt
            })
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<FlashcardItemDto>> GetDeckItemsAsync(
        Guid deckId,
        Guid userId,
        CancellationToken ct = default)
    {
        if (deckId == Guid.Empty)
            throw new ArgumentException("DeckId cannot be empty", nameof(deckId));
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        // Verify ownership to prevent IDOR.
        var isOwned = await _context.Set<FlashcardDeck>()
            .AsNoTracking()
            .AnyAsync(d => d.Id == deckId && d.UserId == userId, ct);

        if (!isOwned)
            throw new UnauthorizedAccessException("Deck not found or access denied.");

        return await _context.Set<FlashcardItem>()
            .AsNoTracking()
            .Where(i => i.DeckId == deckId)
            .OrderBy(i => i.Position)
            .ThenBy(i => i.CreatedAt)
            .Select(i => new FlashcardItemDto
            {
                Id = i.Id,
                Front = i.Front,
                Back = i.Back,
                SourceType = i.SourceType,
                IsFavorite = i.IsFavorite,
                Position = i.Position
            })
            .ToListAsync(ct);
    }
}