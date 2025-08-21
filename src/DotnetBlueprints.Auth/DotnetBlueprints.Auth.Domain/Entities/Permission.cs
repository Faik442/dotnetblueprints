using DotnetBlueprints.SharedKernel.Domain;

namespace DotnetBlueprints.Auth.Domain.Entities;

/// <summary>
/// Represents a permission that can be granted to roles or users.
/// The <see cref="Key"/> is a stable identifier (e.g., "Offers.Read.Company") and should be treated as immutable.
/// Audit fields are inherited from <see cref="BaseEntity"/>.
/// </summary>
public sealed class Permission : BaseEntity
{
    /// <summary>
    /// Parameterless constructor required by EF Core.
    /// </summary>
    private Permission() { }

    /// <summary>
    /// Creates a new <see cref="Permission"/> with the given key and optional description.
    /// </summary>
    /// <param name="key">Stable permission key (e.g., "Offers.Read.Self"). Must be non-empty.</param>
    /// <param name="description">Human-readable description for display purposes.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="key"/> is null or whitespace.</exception>
    public Permission(string key, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Permission key cannot be empty.", nameof(key));

        Key = key;
        Description = description;
    }

    /// <summary>
    /// Gets the stable permission key. This value is intended to be immutable after creation.
    /// </summary>
    public string Key { get; private set; } = default!;

    /// <summary>
    /// Gets the human-readable description of the permission.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the permissions assigned to this role.
    /// </summary>
    public IReadOnlyCollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    /// <summary>
    /// Updates the human-readable description of the permission.
    /// Does not change the stable <see cref="Key"/>.
    /// </summary>
    /// <param name="newDescription">New description text.</param>
    public void Rename(string newDescription, string updatedBy)
    {
        Description = newDescription;
        UpdatedBy = updatedBy;
        UpdatedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the permission as soft deleted.
    /// </summary>
    public void Delete(string deletedBy)
    {
        IsDeleted = true;
        DeletedDate = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
