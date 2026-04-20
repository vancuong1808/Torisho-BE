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

    public async Task<IEnumerable<FlashcardFolderDto>> GetUserFoldersAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        return await _context.Set<FlashcardFolder>()
            .AsNoTracking()
            .Where(f => f.UserId == userId)
            .OrderBy(f => f.DisplayOrder)
            .ThenBy(f => f.CreatedAt)
            .Select(f => new FlashcardFolderDto(
                f.Id,
                f.Name,
                f.Description,
                f.DisplayOrder,
                f.Decks.Count(d => !d.IsArchived),
                f.CreatedAt))
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<FlashcardDeckDto>> GetUserDecksAsync(
        Guid userId,
        Guid? folderId = null,
        CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        if (folderId.HasValue && folderId.Value == Guid.Empty)
            throw new ArgumentException("FolderId cannot be empty", nameof(folderId));

        var query = _context.Set<FlashcardDeck>()
            .AsNoTracking()
            .Where(d => d.UserId == userId && !d.IsArchived);

        if (folderId.HasValue)
            query = query.Where(d => d.FolderId == folderId.Value);

        return await query
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => new FlashcardDeckDto(
                d.Id,
                d.Name,
                d.Description,
                d.FolderId,
                d.Folder != null ? d.Folder.Name : null,
                d.Items.Count,
                d.CreatedAt))
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