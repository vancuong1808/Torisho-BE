using Torisho.Application.DTOs.Dictionary;

namespace Torisho.Application.Services.Dictionary;

public interface IDictionarySearchService
{
    Task<IReadOnlyList<WordSchemaDto>> SearchAsync(string keyword, CancellationToken ct = default);
}
