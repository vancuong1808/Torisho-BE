using Torisho.Application.DTOs.Dictionary;

namespace Torisho.Application.Interfaces.Dictionary;

public interface IDictionarySearchService
{
    Task<IReadOnlyList<WordSchemaDto>> SearchAsync(string keyword, CancellationToken ct = default);
}
