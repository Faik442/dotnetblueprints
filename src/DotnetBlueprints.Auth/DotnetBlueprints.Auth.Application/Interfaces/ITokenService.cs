namespace DotnetBlueprints.Auth.Application.Interfaces;

/// <summary>
/// Issues JWT/refresh tokens for different scopes (bootstrap/company).
/// </summary>
public interface ITokenService
{
    /// <summary>Issues a company-scoped access token (typ=company) and a refresh token.</summary>
    Task<TokenPair> IssueAccessAsync(Guid userId, CancellationToken ct);

    /// <summary>Issues a company-scoped access token (typ=company) and a refresh token.</summary>
    Task<TokenPair> RefreshAsync(string rawRefreshToken, CancellationToken ct);
}

/// <summary>DTO for access + refresh tokens.</summary>
public sealed record TokenPair(string AccessToken, DateTime AccessTokenExpiresAtUtc, string RefreshToken);


