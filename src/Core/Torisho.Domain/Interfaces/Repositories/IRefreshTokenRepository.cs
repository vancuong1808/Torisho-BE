using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task<IEnumerable<RefreshToken>> GetActiveTokensByUserAsync(Guid userId, CancellationToken ct = default);
    Task RevokeAllUserTokensAsync(Guid userId, CancellationToken ct = default);
}
