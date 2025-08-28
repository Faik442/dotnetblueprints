using DotnetBlueprints.Auth.Application.Interfaces;
using DotnetBlueprints.Auth.Domain.Entities;
using DotnetBlueprints.Auth.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

public sealed class TokenService : ITokenService
{
    private readonly IAuthDbContext _db;
    private readonly IOptions<JwtOptions> _opt;

    public TokenService(IAuthDbContext db, IOptions<JwtOptions> opt)
    {
        _db = db; _opt = opt;
    }

    /// <summary>
    /// Issues a new access token (typ=access) and a refresh token for the given user.
    /// The access token contains <c>company_id</c> and one or more <c>role_id</c> claims.
    /// </summary>
    /// <param name="userId">The authenticated user's identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// A <see cref="TokenPair"/> containing the signed access token, its UTC expiry,
    /// and the raw refresh token (only returned once).
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when the user does not exist or has no company assigned.
    /// </exception>
    public async Task<TokenPair> IssueAccessAsync(Guid userId, CancellationToken ct)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, ct)
            ?? throw new UnauthorizedAccessException("User not found.");

        var (access, exp, newJti) = await GenerateAccessToken(user, ct);

        var raw = CreateRawRefreshToken();
        var hash = Hash(raw);
        _db.RefreshTokens.Add(new RefreshToken(user.Id, hash, DateTime.UtcNow.AddDays(_opt.Value.RefreshTokenDays), newJti));
        await _db.SaveChangesAsync(ct);

        return new TokenPair(access, exp, raw);
    }

    /// <summary>
    /// Rotates a valid refresh token to issue a fresh access token and a new refresh token.
    /// Implements one-time use semantics: the provided refresh token is revoked and replaced.
    /// </summary>
    /// <param name="rawRefreshToken">The raw refresh token string provided by the client.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// A <see cref="TokenPair"/> containing a newly signed access token, its UTC expiry,
    /// and a new raw refresh token. The old refresh token is revoked.
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when the refresh token is invalid, revoked, expired, or the user is invalid / has no company.
    /// </exception>
    /// <remarks>
    /// Rotation flow:
    /// 1) Look up the hashed refresh token and validate (not revoked, not expired).<br/>
    /// 2) Revoke the old refresh token and link it to the new JTI.<br/>
    /// 3) Issue a new access token (same user & company context).<br/>
    /// 4) Create and persist a new refresh token tied to the new JTI.
    /// </remarks>
    public async Task<TokenPair> RefreshAsync(string rawRefreshToken, CancellationToken ct)
    {
        var hash = Hash(rawRefreshToken);

        var rt = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Hash == hash, ct)
                 ?? throw new UnauthorizedAccessException("Invalid refresh token.");

        if (rt.RevokedAt is not null) throw new UnauthorizedAccessException("Refresh token revoked.");
        if (DateTime.UtcNow >= rt.ExpiresAt) throw new UnauthorizedAccessException("Refresh token expired.");

        var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Id == rt.UserId && !u.IsDeleted, ct)
                ?? throw new UnauthorizedAccessException("User not found.");

        var newJti = Guid.NewGuid().ToString("N");
        rt.Revoke(newJti);
        await _db.SaveChangesAsync(ct);

        var (access, exp, _) = await GenerateAccessToken(user, ct, jtiOverride: newJti);

        var newRaw = CreateRawRefreshToken();
        var newHash = Hash(newRaw);
        _db.RefreshTokens.Add(new RefreshToken(user.Id, newHash, DateTime.UtcNow.AddDays(_opt.Value.RefreshTokenDays), newJti));
        await _db.SaveChangesAsync(ct);

        return new TokenPair(access, exp, newRaw);
    }

    /// <summary>
    /// Builds and signs a compact access token for the provided user, adding <c>company_id</c>
    /// and all scoped <c>role_id</c> claims. Optionally uses a supplied JTI.
    /// </summary>
    /// <param name="user">The user to issue an access token for.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <param name="jtiOverride">
    /// Optional JTI value to use (e.g., when rotating a refresh token); if null, a new JTI is generated.
    /// </param>
    /// <returns>
    /// Tuple of (access token string, expiration UTC, jti).
    /// </returns>
    private async Task<(string access, DateTime expUtc, string jti)> GenerateAccessToken(
        User user,
        CancellationToken ct,
        string? jtiOverride = null)
    {
        var scopedRoleIds = await
             (from ucr in _db.UserCompanyRoles
              join r in _db.Roles on ucr.RoleId equals r.Id
              where ucr.UserId == user.Id
                 && ucr.CompanyId == user.CompanyId
                 && !r.IsDeleted
                 && (r.CompanyId == user.CompanyId || r.CompanyId == null) 
              select r.Id)
             .Distinct()
             .ToListAsync(ct);

        var exp = DateTime.UtcNow.AddMinutes(_opt.Value.AccessTokenMinutes);
        var jti = jtiOverride ?? Guid.NewGuid().ToString("N");

        var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
        new(ClaimTypes.Email, user.Email ?? string.Empty),
        new(ClaimTypes.Name, user.DisplayName),
        new(JwtRegisteredClaimNames.Jti, jti),
        new("typ","access"),
        new("company_id", user.CompanyId.ToString())
    };

        foreach (var rid in scopedRoleIds)
            claims.Add(new Claim("role_id", rid.ToString()));

        var access = SignJwt(claims, exp);
        return (access, exp, jti);
    }

    private string SignJwt(IEnumerable<Claim> claims, DateTime expUtc)
    {
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_opt.Value.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _opt.Value.Issuer,
            audience: _opt.Value.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expUtc,
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string CreateRawRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    private static string Hash(string raw)
    {
        using var sha = SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(raw)));
    }
}