using DotnetBlueprints.SharedKernel.Domain;

namespace DotnetBlueprints.Auth.Domain.Entities;

/// <summary>
/// Represents a long-lived refresh token used to obtain new access tokens.
/// </summary>
public sealed class RefreshToken : BaseEntity
{
    private RefreshToken() { }

    /// <summary>
    /// Creates a new refresh token.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="tokenHash">Hashed refresh token string (never store raw).</param>
    /// <param name="expiresAt">Expiration time in UTC.</param>
    /// <param name="jwtId">Associated JWT identifier.</param>
    public RefreshToken(Guid userId, string tokenHash, DateTime expiresAt, string jwtId)
    {
        UserId = userId;
        TokenHash = tokenHash ?? throw new ArgumentNullException(nameof(tokenHash));
        ExpiresAt = expiresAt;
        JwtId = jwtId ?? throw new ArgumentNullException(nameof(jwtId));
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the hashed token value.
    /// </summary>
    public string TokenHash { get; private set; }

    /// <summary>
    /// Gets the expiration date (UTC).
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// Gets the creation time (UTC).
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the revocation time, if revoked.
    /// </summary>
    public DateTime? RevokedAt { get; private set; }

    /// <summary>
    /// Gets the associated JWT identifier.
    /// </summary>
    public string JwtId { get; private set; }

    /// <summary>
    /// Marks the refresh token as revoked.
    /// </summary>
    public void Revoke()
    {
        if (RevokedAt is null)
        {
            RevokedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Checks whether the refresh token is expired or revoked.
    /// </summary>
    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
}
