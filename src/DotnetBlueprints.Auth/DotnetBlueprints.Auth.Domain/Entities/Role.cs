using DotnetBlueprints.Auth.Domain.Enums;
using DotnetBlueprints.SharedKernel.Domain;

namespace DotnetBlueprints.Auth.Domain.Entities;

/// <summary>
/// Represents a role that aggregates a set of permissions.
/// A role can be system-wide (CompanyId = null) or scoped to a specific company.
/// Audit and soft-delete fields come from <see cref="BaseEntity"/>.
/// </summary>
public sealed class Role : BaseEntity
{
    private readonly List<RolePermission> _rolePermissions = new();

    /// <summary>
    /// Parameterless constructor required by EF Core.
    /// </summary>
    private Role() { }

    /// <summary>
    /// Creates a new role with the given name and optional company scope.
    /// </summary>
    /// <param name="name">Human-readable role name (e.g., "Admin", "CompanyViewer").</param>
    /// <param name="companyId">If null, the role is system-wide; otherwise, it is company-scoped.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace.</exception>
    public Role(string name, Guid? companyId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty.", nameof(name));

        Name = name;
        CompanyId = companyId;
    }

    /// <summary>
    /// Gets the company scope of the role. Null means the role is system-wide.
    /// </summary>
    public Guid? CompanyId { get; private set; }

    /// <summary>
    /// Gets the role's display name.
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// Indicates whether the role is system-wide (no company scope).
    /// </summary>
    public bool IsSystemRole => CompanyId is null;

    /// <summary>
    /// Gets the permissions assigned to this role.
    /// </summary>
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions;

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
    public void AddPermission(Guid permissionId)
    {
        if (_rolePermissions.Any(rp => rp.PermissionId == permissionId))
            return;

        _rolePermissions.Add(new RolePermission(Id, permissionId));
    }

    /// <summary>
    /// Removes the specified permission to the role if already present.
    /// </summary>
    /// <param name="permission">Permission entity to be granted.</param>
    public void RemovePermission(Guid permissionId)
    {
        if (_rolePermissions.Any(rp => rp.PermissionId == permissionId))
            return;

        _rolePermissions.Remove(new RolePermission(Id, permissionId));
    }

    /// <summary>
    /// Marks this role as soft deleted.
    /// </summary>
    public void Delete() => IsDeleted = true;
}
