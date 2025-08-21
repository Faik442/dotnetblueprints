using DotnetBlueprints.Auth.Domain.Enums;
using DotnetBlueprints.SharedKernel.Abstractions;
using DotnetBlueprints.SharedKernel.Domain;
using System.Security;

namespace DotnetBlueprints.Auth.Domain.Entities;

/// <summary>
/// Represents a company as an aggregate root in the Auth context.
/// Audit fields (CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
/// are inherited from BaseEntity and populated automatically by EF interceptors.
/// </summary>
public sealed class Company : BaseEntity, IAggregateRoot
{
    private readonly List<UserCompany> _members = new();
    private readonly List<Role> _roles = new();

    private Company() { }

    public Company(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Company name cannot be empty.", nameof(name));

        Name = name;
    }

    /// <summary>
    /// Gets the name of the company.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the collection of company members.
    /// </summary>
    public IReadOnlyCollection<UserCompany> Members => _members;

    /// <summary>
    /// Gets the collection of roles defined in the company scope.
    /// </summary>
    public IReadOnlyCollection<Role> Roles => _roles;

    /// <summary>
    /// Factory method for creating a new company instance.
    /// </summary>
    public static Company Create(string name) => new(name);

    /// <summary>
    /// Renames the company.
    /// </summary>
    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Company name cannot be empty.", nameof(newName));

        if (Name.Equals(newName, StringComparison.OrdinalIgnoreCase))
            return;

        Name = newName;
    }

    /// <summary>
    /// Marks the company as soft deleted.
    /// </summary>
    public void Delete(string deletedBy)
    {
        if (string.IsNullOrWhiteSpace(deletedBy))
            throw new ArgumentException("DeletedBy cannot be empty.", nameof(deletedBy));
        
        IsDeleted = true;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Adds a new member to the company with the given roles.
    /// </summary>
    public UserCompany AddMember(User user, IEnumerable<Role> roles, bool isPrimary = false)
    {
        if (_members.Any(m => m.UserId == user.Id))
            throw new InvalidOperationException($"User {user.Email} is already a member of {Name}");

        var membership = new UserCompany(user.Id, Id, isPrimary);
        foreach (var role in roles)
        {
            membership.AddRole(role);
        }

        _members.Add(membership);
        return membership;
    }

    /// <summary>
    /// Adds a new role to the company with the specified permissions.
    /// </summary>
    public Role AddRole(string roleName, IEnumerable<PermissionKey> permissions)
    {
        if (_roles.Any(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Role {roleName} already exists in company {Name}");

        var role = new Role(roleName, Id);
        foreach (var perm in permissions)
        {
            role.AddPermission(new Permission(perm.Value));
        }

        _roles.Add(role);
        return role;
    }
}
