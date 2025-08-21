using DotnetBlueprints.SharedKernel.Domain;

namespace DotnetBlueprints.Auth.Domain.Entities;

/// <summary>
/// Represents a membership link between a user and a company,
/// optionally marked as the user's primary company.
/// Holds role assignments scoped to the company.
/// </summary>
public sealed class UserCompany : BaseEntity
{
    private readonly List<UserCompanyRole> _userCompanyRoles = new();

    /// <summary>
    /// Parameterless constructor required by EF Core.
    /// </summary>
    private UserCompany() { }

    /// <summary>
    /// Creates a new user-company membership.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="companyId">Company identifier.</param>
    /// <param name="isPrimary">Whether this membership is the user's primary one.</param>
    public UserCompany(Guid userId, Guid companyId, bool isPrimary = false)
    {
        UserId = userId;
        CompanyId = companyId;
        IsPrimary = isPrimary;
    }

    /// <summary>
    /// Gets the user identifier (part of the composite key).
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the user navigation.
    /// </summary>
    public User User { get; private set; } = default!;

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
    /// Gets a value indicating whether this membership is the user's primary one.
    /// </summary>
    public bool IsPrimary { get; private set; }

    /// <summary>
    /// Marks this membership as the primary one.
    /// (Caller should ensure only one primary membership per user.)
    /// </summary>
    public void SetPrimary() => IsPrimary = true;

    /// <summary>
    /// Marks this membership is not the primary one.
    /// (Caller should ensure only one primary membership per user.)
    /// </summary>
    public void UnsetPrimary() => IsPrimary = false;

    /// <summary>
    /// Assigns a role if not already present.
    /// </summary>
    public void AddRole(Role role)
    {
        if (role is null) throw new ArgumentNullException(nameof(role));
        if (_userCompanyRoles.Any(r => r.RoleId == role.Id)) return;

        _userCompanyRoles.Add(new UserCompanyRole(UserId, CompanyId, role.Id));
    }

    /// <summary>
    /// Removes a role if present.
    /// </summary>
    public void Delete(Guid roleId, string deletedBy)
    {
        var link = _userCompanyRoles.FirstOrDefault(r => r.RoleId == roleId);
        if (link is null) return;
        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedDate = DateTime.Now;
    }
}
