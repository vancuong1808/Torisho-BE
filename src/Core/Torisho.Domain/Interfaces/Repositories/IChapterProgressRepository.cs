using Torisho.Domain.Entities.ProgressDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IChapterProgressRepository : IRepository<ChapterProgress>
{
    // Get chapter progress for user and chapter
    // Use cases: Display completion percentage, Check unlock status, Update progress, Verify prerequisites
    Task<ChapterProgress?> GetByUserAndChapterAsync(Guid userId, Guid chapterId, CancellationToken ct = default);

    // Get all chapter progress in level
    // Use cases: Level dashboard display, Calculate level completion, Check unlock requirements
    Task<IEnumerable<ChapterProgress>> GetByUserAndLevelAsync(Guid userId, Guid levelId, CancellationToken ct = default);

    // Get all unlocked chapters for user
    // Use cases: "Continue Learning" widget, Quick access menu, Filter navigation, Suggest next chapter
    Task<IEnumerable<ChapterProgress>> GetUnlockedChaptersAsync(Guid userId, CancellationToken ct = default);
}