using Torisho.Application.DTOs.Dictionary.Comment;

namespace Torisho.Application.Interfaces.Dictionary;

public interface IDictionaryCommentService
{
    Task<IReadOnlyList<DictionaryCommentDto>> GetByDictionaryEntryAsync(Guid dictionaryEntryId, CancellationToken ct = default);

    Task<DictionaryCommentDto> CreateAsync(
        Guid dictionaryEntryId,
        Guid userId,
        CreateDictionaryCommentRequest request,
        CancellationToken ct = default);

    Task<DictionaryCommentDto> UpdateAsync(
        Guid dictionaryEntryId,
        Guid commentId,
        Guid userId,
        UpdateDictionaryCommentRequest request,
        CancellationToken ct = default);
}
