using Torisho.Domain.Common;
using Torisho.Domain.Enums;

namespace Torisho.Domain.Entities.UserDomain;

public sealed class User : UserAuth, IAggregateRoot
{
    public string FullName { get; private set; } = default!;
    public UserStatus Status { get; private set; } = UserStatus.Active;
    public string? AvatarUrl { get; private set; }

    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = default!;

    private User() { }

    public User(string fullName, string username, string email, string passwordHash, Guid roleId)
        : base(username, email, passwordHash)
    {
        FullName = fullName;
        RoleId = roleId;
    }

    public void AssignRole(Role role)
    {
        Role = role;
        RoleId = role.Id;
    }

    public override bool HasPermission(string code) => Role.HasPermission(code);
}