using Torisho.Domain.Entities.FlashcardDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IFlashcardFolderRepository : IRepository<FlashcardFolder>
{
    Task<bool> IsOwnedByUserAsync(Guid folderId, Guid userId, CancellationToken ct = default);

    Task<bool> ExistsByUserAndNameAsync(
        Guid userId,
        string name,
        Guid? excludeFolderId = null,
        CancellationToken ct = default);
}
