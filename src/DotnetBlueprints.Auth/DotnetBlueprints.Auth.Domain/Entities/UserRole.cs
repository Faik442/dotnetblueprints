using DotnetBlueprints.SharedKernel.Domain;

namespace DotnetBlueprints.Auth.Domain.Entities;

/// <summary>
/// Join entity linking a user-company membership to a role within the same company scope.
/// Acts as a pure association (no business behavior).
/// Composite key: (UserId, CompanyId, RoleId).
/// </summary>
public sealed class UserRole
{
    /// <summary>
    /// Parameterless constructor required by EF Core.
    /// </summary>
    private UserRole() { }

    /// <summary>
    /// Creates a new user-company-role link.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="roleId">Role identifier (must belong to the same company or be system-wide).</param>
    public UserRole(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }

    /// <summary>
    /// Gets the user identifier (composite key part).
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the role identifier (composite key part).
    /// </summary>
    public Guid RoleId { get; private set; }

    /// <summary>
    /// Navigation to the user-company membership.
    /// </summary>
    public User User { get; private set; } = default!;

    /// <summary>
    /// Navigation to the role.
    /// </summary>
    public Role Role { get; private set; } = default!;
}
