using DotnetBlueprints.SharedKernel.Security;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DotnetBlueprints.Sales.Infrastructure.Identity;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _http;

    public CurrentUser(IHttpContextAccessor http) => _http = http;

    public Guid? UserId =>
        TryParseGuid(_http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier));

    public string? Email =>
        _http.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

    public string? DisplayName =>
        _http.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

    public Guid? CompanyId =>
        TryParseGuid(_http.HttpContext?.User?.FindFirstValue(ClaimNames.CompanyId));

    public IReadOnlyCollection<Guid> RoleIds =>
        _http.HttpContext?.User?
            .FindAll(ClaimNames.RoleId)
            .Select(c => TryParseGuid(c.Value))
            .Where(g => g.HasValue)
            .Select(g => g!.Value)
            .ToArray()
        ?? Array.Empty<Guid>();

    private static Guid? TryParseGuid(string? s)
        => Guid.TryParse(s, out var g) ? g : (Guid?)null;
}

