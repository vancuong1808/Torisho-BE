using System;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<User?> GetWithRolesAsync(Guid userId, CancellationToken ct = default);
    Task<bool> IsEmailExistsAsync(string email, CancellationToken ct = default);
    Task<bool> IsUsernameExistsAsync(string username, CancellationToken ct = default);
}
