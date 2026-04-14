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

        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required", nameof(userId));
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Deck name is required", nameof(request.Name));

        var deck = new FlashcardDeck(
            userId: userId,
            name: request.Name,
            folderId: request.FolderId,
            description: request.Description);

        await _unitOfWork.FlashcardDecks.AddAsync(deck, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return deck.Id;
    }
}