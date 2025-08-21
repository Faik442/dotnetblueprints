using DotnetBlueprints.SharedKernel.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DotnetBlueprints.Auth.Infrastructure.Persistence;

/// <summary>
/// ASP.NET Core implementation of <see cref="IUserContext"/> that reads values from HttpContext.User.
/// </summary>
public sealed class CurrentUserContext : IUserContext
{
    private readonly IHttpContextAccessor _http;

    /// <summary>Initializes a new instance of the <see cref="CurrentUserContext"/> class.</summary>
    public CurrentUserContext(IHttpContextAccessor http) => _http = http;

    /// <inheritdoc />
    public string? UserId =>
        _http.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? _http.HttpContext?.User?.FindFirst("sub")?.Value;

    /// <inheritdoc />
    public string? UserName =>
        _http.HttpContext?.User?.Identity?.Name
        ?? _http.HttpContext?.User?.FindFirst("email")?.Value;
}
