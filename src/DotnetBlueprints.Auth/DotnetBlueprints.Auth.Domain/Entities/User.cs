using DotnetBlueprints.SharedKernel.Abstractions;
using DotnetBlueprints.SharedKernel.Domain;

namespace DotnetBlueprints.Auth.Domain.Entities;

/// <summary>
/// Represents an application user that can belong to one or more companies
/// and hold specific permission overrides.
/// </summary>
public sealed class User : BaseEntity, IAggregateRoot
{
    private readonly List<UserCompany> _companies = new();
    private readonly List<UserPermissionOverride> _permissionOverrides = new();

    private User() { }

    public User(string email, string? displayName = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        Email = email;
        DisplayName = displayName;
        IsActive = true;
        IsDeleted = false;
    }

    public string Email { get; private set; } = default!;
    public string? DisplayName { get; private set; }
    public string? PasswordHash { get; private set; }
    public bool IsActive { get; private set; }

    public IReadOnlyCollection<UserCompany> Companies => _companies;
    public IReadOnlyCollection<UserPermissionOverride> PermissionOverrides => _permissionOverrides;

    /// <summary>
    /// Deactivates the user without deleting it.
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Reactivates the user.
    /// </summary>
    public void Activate() => IsActive = true;

    /// <summary>
    /// Marks the user as deleted (soft delete).
    /// </summary>
    public void MarkAsDeleted(string deletedBy)
    {
        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Changes the user’s display name.
    /// </summary>
    public void ChangeDisplayName(string newName, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Display name cannot be empty", nameof(newName));

        DisplayName = newName;
        UpdatedBy = updatedBy;
        UpdatedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets a new password hash for the user.
    /// </summary>
    public void SetPassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        PasswordHash = passwordHash;
    }

    /// <summary>
    /// Sets primary company of the user.
    /// </summary>
    public void SetPrimaryCompany(Guid companyId)
    {
        if (!_companies.Any(c => c.CompanyId == companyId))
            throw new InvalidOperationException("User is not a member of this company.");

        foreach (var membership in _companies)
        {
            membership.UnsetPrimary();
        }

        _companies.First(c => c.CompanyId == companyId).SetPrimary();
    }
}
