using Torisho.Application.DTOs.Dictionary;

namespace Torisho.Application.Interfaces.Dictionary;

public interface IDictionaryCommentService
{
    Task<IReadOnlyList<DictionaryCommentDto>> GetByDictionaryEntryAsync(Guid dictionaryEntryId, CancellationToken ct = default);

    Task<DictionaryCommentDto> CreateAsync(
        Guid dictionaryEntryId,
        Guid userId,
        CreateDictionaryCommentRequest request,
        CancellationToken ct = default);
}
