using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Domain.Entities.FlashcardDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public sealed class FlashcardFolderRepository : GenericRepository<FlashcardFolder>, IFlashcardFolderRepository
{
    public FlashcardFolderRepository(IDataContext context) : base(context)
    {
    }

    public async Task<bool> IsOwnedByUserAsync(Guid folderId, Guid userId, CancellationToken ct = default)
    {
        if (folderId == Guid.Empty)
            throw new ArgumentException("FolderId cannot be empty", nameof(folderId));
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        return await _dbSet
            .AsNoTracking()
            .AnyAsync(folder => folder.Id == folderId && folder.UserId == userId, ct);
    }

    public async Task<bool> ExistsByUserAndNameAsync(
        Guid userId,
        string name,
        Guid? excludeFolderId = null,
        CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Folder name is required", nameof(name));

        var normalizedName = name.Trim();

        return await _dbSet
            .AsNoTracking()
            .AnyAsync(
                folder => folder.UserId == userId &&
                          folder.Name == normalizedName &&
                          (!excludeFolderId.HasValue || folder.Id != excludeFolderId.Value),
                ct);
    }
}
