using Torisho.Domain.Common;

namespace Torisho.Domain.Entities.UserDomain;

public sealed class Role : BaseEntity
{
    // DDD: Aggregate - Role manages Permissions through domain methods
    private readonly HashSet<Permission> _permissions = new();
    public IReadOnlyCollection<Permission> Permissions => _permissions;

    public string? Name { get; private set; }
    public string? Description { get; private set; }


    private Role() { }

    public Role(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public void AddPermission(Permission permission)
    {
        ArgumentNullException.ThrowIfNull(permission);
        _permissions.Add(permission);
    }

    public void RemovePermission(Permission permission)
    {
        ArgumentNullException.ThrowIfNull(permission);
        _permissions.Remove(permission);
    }

    public bool HasPermission(string code) => _permissions.Any(p => p.Code == code);
}
