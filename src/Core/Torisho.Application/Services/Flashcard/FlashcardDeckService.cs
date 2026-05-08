using Torisho.Application.DTOs.Flashcard;
using Torisho.Application.Interfaces.Flashcard;
using Torisho.Domain.Entities.FlashcardDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Application.Services.Flashcard;

public sealed class FlashcardDeckService : IFlashcardDeckService
{
    private readonly IUnitOfWork _unitOfWork;

    public FlashcardDeckService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Guid> CreateAsync(Guid userId, CreateFlashcardDeckRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateId(userId, nameof(userId));
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Deck name is required", nameof(request.Name));

        if (request.FolderId.HasValue)
        {
            var folderExists = await _unitOfWork.FlashcardFolders
                .IsOwnedByUserAsync(request.FolderId.Value, userId, ct);

            if (!folderExists)
                throw new KeyNotFoundException("Folder not found.");
        }

        var deck = new FlashcardDeck(
            userId: userId,
            name: request.Name,
            folderId: request.FolderId,
            description: request.Description);

        await _unitOfWork.FlashcardDecks.AddAsync(deck, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return deck.Id;
    }

    public async Task UpdateAsync(Guid userId, Guid deckId, UpdateFlashcardDeckRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateId(userId, nameof(userId));
        ValidateId(deckId, nameof(deckId));

        var deck = await _unitOfWork.FlashcardDecks.GetByIdAsync(deckId, ct);

        if (deck is null || deck.UserId != userId || deck.IsArchived)
            throw new KeyNotFoundException("Deck not found.");

        if (request.FolderId.HasValue)
        {
            var folderExists = await _unitOfWork.FlashcardFolders
                .IsOwnedByUserAsync(request.FolderId.Value, userId, ct);

            if (!folderExists)
                throw new KeyNotFoundException("Folder not found.");
        }

        deck.UpdateDetails(request.Name, request.Description);
        deck.SetFolder(request.FolderId);

        await _unitOfWork.FlashcardDecks.UpdateAsync(deck, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid userId, Guid deckId, CancellationToken ct = default)
    {
        ValidateId(userId, nameof(userId));
        ValidateId(deckId, nameof(deckId));

        var deck = await _unitOfWork.FlashcardDecks.GetByIdAsync(deckId, ct);

        if (deck is null || deck.UserId != userId || deck.IsArchived)
            throw new KeyNotFoundException("Deck not found.");

        deck.SetArchived(true);

        await _unitOfWork.FlashcardDecks.UpdateAsync(deck, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private static void ValidateId(Guid id, string paramName)
    {
        if (id == Guid.Empty)
            throw new ArgumentException($"{paramName} is required", paramName);
    }
}