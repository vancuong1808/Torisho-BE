using Torisho.Application.DTOs.Flashcard;
using Torisho.Application.Interfaces.Flashcard;
using Torisho.Domain.Entities.FlashcardDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Application.Services.Flashcard;

public sealed class FlashcardFolderService : IFlashcardFolderService
{
    private readonly IUnitOfWork _unitOfWork;

    public FlashcardFolderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Guid> CreateAsync(Guid userId, CreateFlashcardFolderRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateId(userId, nameof(userId));

        var normalizedName = NormalizeRequiredName(request.Name, nameof(request.Name));

        var hasDuplicate = await _unitOfWork.FlashcardFolders
            .ExistsByUserAndNameAsync(userId, normalizedName, null, ct);

        if (hasDuplicate)
            throw new ArgumentException("Folder name already exists.", nameof(request.Name));

        var folder = new FlashcardFolder(
            userId: userId,
            name: normalizedName,
            description: request.Description,
            displayOrder: request.DisplayOrder);

        await _unitOfWork.FlashcardFolders.AddAsync(folder, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return folder.Id;
    }

    public async Task UpdateAsync(Guid userId, Guid folderId, UpdateFlashcardFolderRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateId(userId, nameof(userId));
        ValidateId(folderId, nameof(folderId));

        var folder = await _unitOfWork.FlashcardFolders.GetByIdAsync(folderId, ct);

        if (folder is null || folder.UserId != userId)
            throw new KeyNotFoundException("Folder not found.");

        var normalizedName = NormalizeRequiredName(request.Name, nameof(request.Name));

        var hasDuplicate = await _unitOfWork.FlashcardFolders
            .ExistsByUserAndNameAsync(userId, normalizedName, folderId, ct);

        if (hasDuplicate)
            throw new ArgumentException("Folder name already exists.", nameof(request.Name));

        folder.UpdateDetails(normalizedName, request.Description, request.DisplayOrder);

        await _unitOfWork.FlashcardFolders.UpdateAsync(folder, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid userId, Guid folderId, CancellationToken ct = default)
    {
        ValidateId(userId, nameof(userId));
        ValidateId(folderId, nameof(folderId));

        var folder = await _unitOfWork.FlashcardFolders.GetByIdAsync(folderId, ct);

        if (folder is null || folder.UserId != userId)
            throw new KeyNotFoundException("Folder not found.");

        await _unitOfWork.FlashcardFolders.DeleteAsync(folder, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task AddDeckAsync(Guid userId, Guid folderId, Guid deckId, CancellationToken ct = default)
    {
        ValidateId(userId, nameof(userId));
        ValidateId(folderId, nameof(folderId));
        ValidateId(deckId, nameof(deckId));

        var folderExists = await _unitOfWork.FlashcardFolders.IsOwnedByUserAsync(folderId, userId, ct);

        if (!folderExists)
            throw new KeyNotFoundException("Folder not found.");

        var deck = await _unitOfWork.FlashcardDecks.GetByIdAsync(deckId, ct);

        if (deck is null || deck.UserId != userId)
            throw new KeyNotFoundException("Deck not found.");

        deck.SetFolder(folderId);
        await _unitOfWork.FlashcardDecks.UpdateAsync(deck, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task RemoveDeckAsync(Guid userId, Guid folderId, Guid deckId, CancellationToken ct = default)
    {
        ValidateId(userId, nameof(userId));
        ValidateId(folderId, nameof(folderId));
        ValidateId(deckId, nameof(deckId));

        var folderExists = await _unitOfWork.FlashcardFolders.IsOwnedByUserAsync(folderId, userId, ct);

        if (!folderExists)
            throw new KeyNotFoundException("Folder not found.");

        var deck = await _unitOfWork.FlashcardDecks.GetByIdAsync(deckId, ct);

        if (deck is null || deck.UserId != userId || deck.FolderId != folderId)
            throw new KeyNotFoundException("Deck not found in this folder.");

        deck.SetFolder(null);
        await _unitOfWork.FlashcardDecks.UpdateAsync(deck, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    // Helper
    private static void ValidateId(Guid id, string paramName)
    {
        if (id == Guid.Empty)
            throw new ArgumentException($"{paramName} is required", paramName);
    }

    private static string NormalizeRequiredName(string? name, string paramName)
    {
        var normalizedName = name?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(normalizedName))
            throw new ArgumentException("Folder name is required", paramName);

        return normalizedName;
    }
}
