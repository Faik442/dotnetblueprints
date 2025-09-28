using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotnetBlueprints.SharedKernel.Security;

/// <summary>
/// MVC authorization filter that enforces <see cref="RequirePermissionAttribute"/> (ALL-of).
/// - Reads CompanyId and RoleIds from ICurrentUser.
/// - Fetches role→permissions from Redis-backed IRolePermissionCache (cache-only).
/// - Unions permissions of all roles and verifies ALL required permissions are present.
/// Works across ASP.NET Core versions by trying endpoint metadata first, then falling back to reflection.
/// </summary>
public sealed class PermissionAuthorizeFilter : IAsyncAuthorizationFilter
{
    private readonly ICurrentUser _currentUser;
    private readonly IRolePermissionCache _cache;

    public PermissionAuthorizeFilter(ICurrentUser currentUser, IRolePermissionCache cache)
    {
        _currentUser = currentUser;
        _cache = cache;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // --- Collect required permissions (endpoint metadata → reflection fallback) ---
        var requiredAttrs = GetRequiredPermissions(context);
        if (requiredAttrs.Count == 0)
            return;

        // --- Auth pre-checks ---
        if (_currentUser.UserId is null)
        {
            context.Result = new ObjectResult(new { title = "Unauthorized", status = 401 })
            { StatusCode = StatusCodes.Status401Unauthorized };
            return;
        }

        if (_currentUser.CompanyId is null || _currentUser.RoleIds is null || _currentUser.RoleIds.Count == 0)
        {
            context.Result = new ObjectResult(new { title = "Forbidden", status = 403 })
            { StatusCode = StatusCodes.Status403Forbidden };
            return;
        }

        var companyId = _currentUser.CompanyId.Value;
        var ct = context.HttpContext.RequestAborted;

        // --- Build effective permission set by unioning role permissions from cache (strict cache-only) ---
        var tasks = _currentUser.RoleIds.Select(roleId => _cache.GetPermissionsAsync(roleId, ct));
        var sets = await Task.WhenAll(tasks);

        var effective = sets
            .Where(s => s is { Count: > 0 })
            .SelectMany(s => s!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // for admin role
        if (effective.Contains("*")) return;
        
        foreach (var set in sets)
        {
            if (set is null || set.Count == 0) continue;
            foreach (var p in set) effective.Add(p);
        }

        // --- ALL-of check ---
        var required = requiredAttrs.Select(a => a.Permission);
        var allowed = required.All(r => effective.Contains(r));

        if (!allowed)
        {
            context.Result = new ObjectResult(new { title = "Forbidden", status = 403 })
            { StatusCode = StatusCodes.Status403Forbidden };
        }
    }

    /// <summary>
    /// Tries to read RequirePermissionAttribute from endpoint metadata first; if unavailable, falls back to reflection.
    /// </summary>
    private static IReadOnlyList<RequirePermissionAttribute> GetRequiredPermissions(AuthorizationFilterContext context)
    {
        // Preferred: endpoint metadata (works on recent ASP.NET Core)
        var endpoint = context.HttpContext.GetEndpoint();
        if (endpoint is not null)
        {
            var meta = endpoint.Metadata.GetOrderedMetadata<RequirePermissionAttribute>();
            if (meta is { Count: > 0 })
                return meta;
        }

        // Fallback: reflection on controller/action (works on older versions)
        if (context.ActionDescriptor is ControllerActionDescriptor cad)
        {
            var m = cad.MethodInfo
                .GetCustomAttributes(typeof(RequirePermissionAttribute), inherit: true)
                .Cast<RequirePermissionAttribute>();

            var c = cad.ControllerTypeInfo
                .GetCustomAttributes(typeof(RequirePermissionAttribute), inherit: true)
                .Cast<RequirePermissionAttribute>();

            return m.Concat(c).ToArray();
        }

        return Array.Empty<RequirePermissionAttribute>();
    }
}