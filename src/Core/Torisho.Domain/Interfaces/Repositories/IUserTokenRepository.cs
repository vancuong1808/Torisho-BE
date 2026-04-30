using Torisho.Domain.Entities.UserDomain;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IUserTokenRepository : IRepository<UserToken>
{
    Task<UserToken?> GetByTokenHashAsync(string tokenHash, UserTokenType type, CancellationToken ct = default);
    Task InvalidateAllAsync(Guid userId, UserTokenType type, CancellationToken ct = default);
}
