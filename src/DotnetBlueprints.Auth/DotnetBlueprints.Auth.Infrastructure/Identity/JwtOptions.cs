namespace DotnetBlueprints.Auth.Infrastructure.Identity;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    /// <summary>Issuer (iss) claim.</summary>
    public string Issuer { get; set; } = default!;
    /// <summary>Audience (aud) claim.</summary>
    public string[] Audience { get; set; } = default!;
    /// <summary>Signing key (symmetric).</summary>
    public string SigningKey { get; set; } = default!;
    /// <summary>Access token lifetime (minutes).</summary>
    public int AccessTokenMinutes { get; set; } = 15;
    /// <summary>Refresh token lifetime (days).</summary>
    public int RefreshTokenDays { get; set; } = 15;
}

