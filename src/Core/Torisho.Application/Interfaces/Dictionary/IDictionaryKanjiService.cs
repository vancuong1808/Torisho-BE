using System.Threading;
using System.Threading.Tasks;
using Torisho.Application.DTOs.Dictionary;

namespace Torisho.Application.Interfaces.Dictionary;

public interface IDictionaryKanjiService
{
    Task<DictionaryKanjiDetailDto?> GetKanjiDetailAsync(string character, CancellationToken ct = default);
}
