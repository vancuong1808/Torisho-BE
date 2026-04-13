using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.DictionaryDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IDictionaryKanjiRepository : IRepository<Kanji>
{
    Task<Kanji?> GetByCharacterWithRelatedAsync(string character, CancellationToken ct = default);
}