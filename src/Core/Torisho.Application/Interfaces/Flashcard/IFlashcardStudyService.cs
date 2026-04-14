using Torisho.Application.DTOs.Flashcard;

namespace Torisho.Application.Interfaces.Flashcard;

public interface IFlashcardStudyService
{
    Task<Guid> AddFromDictionaryAsync(Guid userId, AddFromDictionaryRequest request, CancellationToken ct = default);

    Task<int> BulkImportAsync(Guid userId, BulkImportRequest request, CancellationToken ct = default);
}