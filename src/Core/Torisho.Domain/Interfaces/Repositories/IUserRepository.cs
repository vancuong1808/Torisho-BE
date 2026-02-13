using System;
using System.Threading;
using System.Threading.Tasks;
using Torisho.Domain.Entities.UserDomain;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    // Get user by email
    // Use cases: Login authentication, Password reset, Profile lookup
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

    // Get user by username
    // Use cases: Login authentication, Public profile display, Validate @mentions
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);

    // Get user with roles and permissions (eager loading)
    // Use cases: Authorization check, Admin panel display, Access control validation
    Task<User?> GetWithRolesAsync(Guid userId, CancellationToken ct = default);

    // Check if email exists
    // Use cases: Registration validation, Profile update validation, Real-time email check
    Task<bool> IsEmailExistsAsync(string email, CancellationToken ct = default);

    // Check if username exists
    // Use cases: Registration validation, Profile update validation, Real-time username check
    Task<bool> IsUsernameExistsAsync(string username, CancellationToken ct = default);
}
