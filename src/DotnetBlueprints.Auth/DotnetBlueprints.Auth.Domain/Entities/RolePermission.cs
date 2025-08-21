namespace DotnetBlueprints.Auth.Domain.Entities;

/// <summary>
/// Join entity that represents the association between a <see cref="Role"/> and a <see cref="Permission"/>.
/// This entity does not have its own business behavior; it exists to model the many-to-many relationship.
/// </summary>
public sealed class RolePermission
{
    /// <summary>
    /// Parameterless constructor required by EF Core.
    /// </summary>
    private RolePermission() { }

    /// <summary>
    /// Creates a new association between a role and a permission.
    /// </summary>
    /// <param name="roleId">The role identifier.</param>
    /// <param name="permissionId">The permission identifier.</param>
    public RolePermission(Guid roleId, Guid permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }

    /// <summary>
    /// Gets the identifier of the role.
    /// </summary>
    public Guid RoleId { get; private set; }

    /// <summary>
    /// Gets the role navigation property.
    /// </summary>
    public Role Role { get; private set; } = default!;

    /// <summary>
    /// Gets the identifier of the permission.
    /// </summary>
    public Guid PermissionId { get; private set; }

    /// <summary>
    /// Gets the permission navigation property.
    /// </summary>
    public Permission Permission { get; private set; } = default!;
}
