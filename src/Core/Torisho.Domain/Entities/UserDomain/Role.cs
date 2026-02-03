using Torisho.Domain.Common;

namespace Torisho.Domain.Entities.UserDomain;

public sealed class Role : BaseEntity
{
    private readonly HashSet<Permission> _permissions = new();

    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;

    public IReadOnlyCollection<Permission> Permissions => _permissions;

    private Role() { }

    public Role(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public void AddPermission(Permission permission)
    {
        if (!_permissions.Contains(permission))
        {
            _permissions.Add(permission);
        }
    }

    public void RemovePermission(Permission permission)
    {
        if (_permissions.Contains(permission))
        {
            _permissions.Remove(permission);
        }
    }

    public bool HasPermission(string code) => _permissions.Any(p => p.Code == code);
}
