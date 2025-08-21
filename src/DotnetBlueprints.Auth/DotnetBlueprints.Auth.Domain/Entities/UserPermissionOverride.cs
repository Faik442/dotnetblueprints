namespace DotnetBlueprints.Auth.Domain.Entities;

/// <summary>
/// Represents a user-specific override for a permission.
/// Can allow or deny a permission regardless of the assigned roles.
/// </summary>
public sealed class UserPermissionOverride
{
    /// <summary>
    /// Parameterless constructor required by EF Core.
    /// </summary>
    private UserPermissionOverride() { }

    /// <summary>
    /// Creates a new user permission override.
    /// </summary>
    /// <param name="userId">The identifier of the user this override applies to.</param>
    /// <param name="permissionKey">The permission key being overridden.</param>
    /// <param name="effect">The effect of the override (allow/deny).</param>
    /// <param name="companyId">
    /// Optional company context. Null means the override applies system-wide.
    /// </param>
    public UserPermissionOverride(Guid userId, string permissionKey, OverrideEffect effect, Guid? companyId = null)
    {
        if (string.IsNullOrWhiteSpace(permissionKey))
            throw new ArgumentException("Permission key cannot be empty.", nameof(permissionKey));

        UserId = userId;
        PermissionKey = permissionKey;
        Effect = effect;
        CompanyId = companyId;
    }

    /// <summary>
    /// Gets the unique identifier of this override entry.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the identifier of the user this override applies to.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the optional company context.
    /// Null means the override is global (applies in all companies).
    /// </summary>
    public Guid? CompanyId { get; private set; }

    /// <summary>
    /// Gets the permission key being overridden.
    /// </summary>
    public string PermissionKey { get; private set; } = default!;

    /// <summary>
    /// Gets the effect of the override (allow or deny).
    /// </summary>
    public OverrideEffect Effect { get; private set; }

    /// <summary>
    /// Updates the override effect (allow/deny).
    /// </summary>
    public void ChangeEffect(OverrideEffect newEffect)
    {
        Effect = newEffect;
    }
}

/// <summary>
/// Represents the effect of a user permission override.
/// </summary>
public enum OverrideEffect
{
    Allow = 1,
    Deny = 2
}
