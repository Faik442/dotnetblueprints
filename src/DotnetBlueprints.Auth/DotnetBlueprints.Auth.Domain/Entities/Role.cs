using DotnetBlueprints.SharedKernel.Domain;

namespace DotnetBlueprints.Auth.Domain.Entities;

/// <summary>
/// Represents a role that aggregates a set of permissions.
/// Audit and soft-delete fields come from <see cref="BaseEntity"/>.
/// </summary>
public sealed class Role : BaseEntity
{
    private readonly List<RolePermission> _rolePermissions = new();
    private readonly List<UserRole> _userRoles = new();

    /// <summary>
    /// Parameterless constructor required by EF Core.
    /// </summary>
    private Role() { }

    /// <summary>
    /// Creates a new role with the given name and optional company scope.
    /// </summary>
    /// <param name="name">Human-readable role name (e.g., "Admin", "CompanyViewer").</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace.</exception>
    public Role(string name, IEnumerable<Guid> permissionIds)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty.", nameof(name));

        Name = name;
        AddPermissions(permissionIds);
        
    }

    /// <summary>
    /// Gets the role's display name.
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// Gets the permissions assigned to this role.
    /// </summary>
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions;

    /// <summary>
    /// Gets the roles assigned to the user within the company scope.
    /// </summary>
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;

    /// <summary>
    /// Renames the role.
    /// </summary>
    /// <param name="newName">New role name.</param>
    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Role name cannot be empty.", nameof(newName));

        if (Name.Equals(newName, StringComparison.OrdinalIgnoreCase))
            return;

        Name = newName;
    }

    /// <summary>
    /// Grants the specified permission to the role if not already present.
    /// </summary>
    /// <param name="permission">Permission entity to be granted.</param>
    public void AddPermissions(IEnumerable<Guid> permissionIds)
    {
        if (permissionIds is null) return;

        foreach (var permissionId in permissionIds.Distinct())
        {
            if (_rolePermissions.Any(rp => rp.PermissionId == permissionId))
                continue;

            _rolePermissions.Add(new RolePermission(Id, permissionId));
        }
    }

    /// <summary>
    /// Deletes the specified permission to the role if not already present.
    /// </summary>
    /// <param name="permission">Permission entity to be deleted.</param>
    public void RemovePermissions(IEnumerable<Guid> permissionIds)
    {
        if (permissionIds is null) return;

        foreach (var permissionId in permissionIds)
        {
            if (!_rolePermissions.Any(rp => rp.PermissionId == permissionId))
                continue;

            var rolePerm = _rolePermissions.FindIndex(x => x.PermissionId == permissionId && x.RoleId == Id);
            _rolePermissions.RemoveAt(rolePerm);
        }
    }

    /// <summary>
    /// Marks this role as soft deleted.
    /// </summary>
    public void Delete() => IsDeleted = true;
}
