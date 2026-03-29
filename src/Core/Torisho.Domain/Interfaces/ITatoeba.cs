using Torisho.Domain.Entities.DictionaryDomain;

namespace Torisho.Domain.Interfaces;
public interface ITatoeba
{
    Task<List<DictionaryExample>> GetExamplesAsync(string keyword, CancellationToken ct = default);
}