using DotnetBlueprints.Auth.Domain.Enums;
using DotnetBlueprints.SharedKernel.Domain;
using DotnetBlueprints.SharedKernel.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;

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
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace.</exception>
    public Role(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty.", nameof(name));

        Name = name;
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
        foreach (var permissionId in permissionIds)
        {
            if (_rolePermissions.Any(rp => rp.PermissionId == permissionId))
                return;

            _rolePermissions.Add(new RolePermission(Id, permissionId));
        }
    }

    /// <summary>
    /// Marks this role as soft deleted.
    /// </summary>
    public void Delete() => IsDeleted = true;
}
