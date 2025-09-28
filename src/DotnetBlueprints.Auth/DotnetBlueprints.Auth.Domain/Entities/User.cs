using DotnetBlueprints.SharedKernel.Abstractions;
using DotnetBlueprints.SharedKernel.Domain;
using StackExchange.Redis;
using System.Data;

namespace DotnetBlueprints.Auth.Domain.Entities;

/// <summary>
/// Represents an application user that can belong to one or more companies
/// and hold specific permission overrides.
/// </summary>
public sealed class User : BaseEntity, IAggregateRoot
{
    private readonly List<UserRole> _userRoles = new();

    private User() { }
    public User(Guid companyId, string email, string displayName, string passwordHash, IEnumerable<Role> roles)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));

        AddRoles(roles);

        CompanyId = companyId;
        Email = email;
        DisplayName = displayName;
        PasswordHash = passwordHash;
        IsDeleted = false;
    }

    /// <summary>Factory method to create a new user with a precomputed password hash.</summary>
    public static User Create(Guid companyId, string email, string displayName, string passwordHash, IEnumerable<Role> roles)
        => new User(companyId, email, displayName, passwordHash, roles);

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
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;

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
    /// Assigns roles if not already present.
    /// </summary>
    public void AddRoles(IEnumerable<Role> roles)
    {
        ArgumentNullException.ThrowIfNull(roles);

        foreach(var role in roles)
        {
            _userRoles.Add(new UserRole(Id, role.Id));
        }
    }
}
