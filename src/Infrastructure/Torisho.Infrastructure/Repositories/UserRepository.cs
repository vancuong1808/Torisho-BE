using Microsoft.EntityFrameworkCore;
using Torisho.Application;
using Torisho.Domain.Entities.UserDomain;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(IDataContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        if(string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email can't be empty!", nameof(email));

        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        if(string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username can't be empty!", nameof(username));

        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username, ct);
    }

    public async Task<User?> GetWithRolesAsync(Guid userId, CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        return await _dbSet
            .Include(u => u.Roles)
                .ThenInclude(ur => ur.Permissions) 
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
    }

    public async Task<bool> IsEmailExistsAsync(string email, CancellationToken ct = default)
    {
        if(string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email can't be empty!", nameof(email));

        return await _dbSet
            .AsNoTracking()
            .AnyAsync(u => u.Email == email, ct);
    }

    public async Task<bool> IsUsernameExistsAsync(string username, CancellationToken ct = default)
    {
        if(string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username can't be empty!", nameof(username));

        return await _dbSet
            .AsNoTracking()
            .AnyAsync(u => u.Username == username, ct);
    }
}