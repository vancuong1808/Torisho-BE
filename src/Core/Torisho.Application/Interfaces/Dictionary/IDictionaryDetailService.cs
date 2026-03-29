using Torisho.Application.DTOs.Dictionary;
namespace Torisho.Application.Interfaces.Dictionary;
public interface IDictionaryDetailService
{
    Task<WordDetailDto?> GetWordDetailAsync(Guid id, CancellationToken ct = default);
}