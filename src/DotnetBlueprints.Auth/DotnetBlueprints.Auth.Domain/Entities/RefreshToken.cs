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
    /// <param name="issuedForJti">The JTI of the access token this refresh token was issued for.</param>
    public RefreshToken(Guid userId, string hash, DateTime expiresAtUtc, string issuedForJti)
    {
        UserId = userId;
        Hash = !string.IsNullOrWhiteSpace(hash)
            ? hash
            : throw new ArgumentNullException(nameof(hash));

        ExpiresAt = expiresAtUtc;

        IssuedForJti = !string.IsNullOrWhiteSpace(issuedForJti)
            ? issuedForJti
            : throw new ArgumentNullException(nameof(issuedForJti));

        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// SHA256 hash of the raw refresh token string (never store raw values).
    /// </summary>
    public string Hash { get; private set; }

    /// <summary>
    /// Gets the expiration date (UTC).
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// The JTI (unique identifier) of the access token that this refresh token was originally issued for.
    /// Used to correlate refresh tokens with access tokens.
    /// </summary>
    public string IssuedForJti { get; private set; } = default!;

    /// <summary>
    /// Gets the creation time (UTC).
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the revocation time, if revoked.
    /// </summary>
    public DateTime? RevokedAt { get; private set; }

    /// <summary>
    /// JTI of the replacement refresh token, if this token was rotated.
    /// Used for traceability and replay detection.
    /// </summary>
    public string? ReplacedByJti { get; private set; }

    /// <summary>
    /// Marks the refresh token as revoked.
    /// </summary>
    public void Revoke(string replacedByJti)
    {
        RevokedAt = DateTime.UtcNow;
        ReplacedByJti = replacedByJti;
    }

    /// <summary>
    /// Checks whether the refresh token is expired or revoked.
    /// </summary>
    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
}
