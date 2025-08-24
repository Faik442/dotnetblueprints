using DotnetBlueprints.SharedKernel.Abstractions;
using DotnetBlueprints.SharedKernel.Domain;
using System.Data;

namespace DotnetBlueprints.Auth.Domain.Entities;

/// <summary>
/// Represents an application user that can belong to one or more companies
/// and hold specific permission overrides.
/// </summary>
public sealed class User : BaseEntity, IAggregateRoot
{
    private readonly List<UserCompanyRole> _userCompanyRoles = new();

    private User(string email, string displayName, string passwordHash, IEnumerable<Role> roles)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));

        foreach (var role in roles)
        {
            AddRole(role);
        }

        Email = email;
        DisplayName = displayName;
        PasswordHash = passwordHash;
        IsDeleted = false;
    }

    /// <summary>Factory method to create a new user with a precomputed password hash.</summary>
    public static User Create(string email, string displayName, string passwordHash, IEnumerable<Role> roles)
        => new User(email, displayName, passwordHash, roles);

    public string Email { get; private set; } = default!;
    public string DisplayName { get; private set; }
    public string PasswordHash { get; private set; }

    /// <summary>
    /// Gets the company identifier (part of the composite key).
    /// </summary>
    public Guid CompanyId { get; private set; }

    /// <summary>
    /// Gets the company navigation.
    /// </summary>
    public Company Company { get; private set; } = default!;

    /// <summary>
    /// Gets the roles assigned to the user within the company scope.
    /// </summary>
    public IReadOnlyCollection<UserCompanyRole> UserCompanyRoles => _userCompanyRoles;

    /// <summary>
    /// Marks the user as deleted (soft delete).
    /// </summary>
    public void Delete() => IsDeleted = true;

    /// <summary>
    /// Changes the user’s display name.
    /// </summary>
    public void ChangeDisplayName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Display name cannot be empty", nameof(newName));

        DisplayName = newName;
    }

    /// <summary>
    /// Changes the user’s email.
    /// </summary>
    public void ChangeEmail(string newMail)
    {
        if (string.IsNullOrWhiteSpace(newMail))
            throw new ArgumentException("Email name cannot be empty", nameof(newMail));

        Email = newMail;
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
    /// Assigns a role if not already present.
    /// </summary>
    public void AddRole(Role role)
    {
        if (role is null) throw new ArgumentNullException(nameof(role));

        var sameCompanyOrSystem = role.CompanyId is null || role.CompanyId == CompanyId;
        if (!sameCompanyOrSystem)
            throw new InvalidOperationException(
                $"Role '{role.Name}' does not belong to company {CompanyId}.");

        if (_userCompanyRoles.Any(r => r.RoleId == role.Id)) return;

        _userCompanyRoles.Add(new UserCompanyRole(Id, CompanyId, role.Id));
    }

    /// <summary>
    /// Removes a role if present.
    /// </summary>
    public void DeleteRole(Guid roleId)
    {
        var link = _userCompanyRoles.FirstOrDefault(r => r.RoleId == roleId);
        if (link is null) return;
        IsDeleted = true;
    }
}
