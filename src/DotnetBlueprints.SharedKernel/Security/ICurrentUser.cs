namespace DotnetBlueprints.SharedKernel.Security;

/// <summary>
/// Provides access to the current authenticated user information
/// required for auditing (CreatedBy/UpdatedBy/DeletedBy).
/// </summary>
public interface ICurrentUser
{
    /// <summary>Gets the stable user identifier (e.g., subject claim).</summary>
    Guid? UserId { get; }

    /// <summary>Gets a human-readable user name (e.g., email or username).</summary>
    string DisplayName { get; }

    /// <summary>Gets the stable company identifier (e.g., subject claim).</summary>
    Guid? CompanyId { get; }

    /// <summary>Gets the user's role ids (e.g., subject claim).</summary>
    IReadOnlyCollection<Guid> RoleIds { get; }

    /// <summary>Gets the user's email (e.g., subject claim).</summary>
    string? Email { get; }
}

