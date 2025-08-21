using DotnetBlueprints.SharedKernel.Domain;

namespace DotnetBlueprints.Auth.Domain.Entities;

/// <summary>
/// Represents a short-lived access token (JWT).
/// Normally not persisted, but can be tracked for auditing or blacklisting.
/// </summary>
public sealed class AccessToken : BaseEntity
{
    private AccessToken() { }

    /// <summary>
    /// Creates a new access token.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="jwt">The raw JWT string.</param>
    /// <param name="expiresAt">Expiration time in UTC.</param>
    /// <param name="jwtId">Unique identifier inside JWT (jti claim).</param>
    public AccessToken(Guid userId, string jwt, DateTime expiresAt, string jwtId)
    {
        UserId = userId;
        Jwt = jwt ?? throw new ArgumentNullException(nameof(jwt));
        ExpiresAt = expiresAt;
        JwtId = jwtId ?? throw new ArgumentNullException(nameof(jwtId));
    }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the raw JWT string.
    /// </summary>
    public string Jwt { get; private set; }

    /// <summary>
    /// Gets the expiration date (UTC).
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// Gets the unique JWT identifier (jti claim).
    /// </summary>
    public string JwtId { get; private set; }

    /// <summary>
    /// Checks whether the token is expired.
    /// </summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}
