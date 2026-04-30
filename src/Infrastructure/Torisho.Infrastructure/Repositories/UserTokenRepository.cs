using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Domain.Entities.UserDomain;
using Torisho.Domain.Enums;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public class UserTokenRepository : GenericRepository<UserToken>, IUserTokenRepository
{
    public UserTokenRepository(IDataContext context) : base(context)
    {
    }

    public async Task<UserToken?> GetByTokenHashAsync(string tokenHash, UserTokenType type, CancellationToken ct = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(ut => ut.TokenHash == tokenHash && ut.Type == type, ct);
    }

    public async Task InvalidateAllAsync(Guid userId, UserTokenType type, CancellationToken ct = default)
    {
        var tokens = await _dbSet
            .Where(ut => ut.UserId == userId && ut.Type == type && !ut.IsUsed && ut.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(ct);

        foreach (var token in tokens)
        {
            token.MarkUsed();
        }
    }
}
