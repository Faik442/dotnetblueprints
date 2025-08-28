using DotnetBlueprints.SharedKernel.Security;
using StackExchange.Redis;
using System.Text.Json;

namespace DotnetBlueprints.SharedKernel.Redis;

/// <summary>
/// Redis-backed cache that stores all role→permissions entries under a single Hash key "Permissions".
/// Field key format: "{companyId}-{roleId}" (GUIDs in "D" format), value: JSON array of strings.
/// Note: Redis does not support per-field TTL; expiration can only be applied to the entire "Permissions" key.
/// </summary>
public sealed class RedisRolePermissionHashCache : IRolePermissionCache
{
    private readonly IDatabase _db;

    // Single hash key requested by the user.
    private const string HashKey = "Permissions";

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = false
    };

    public RedisRolePermissionHashCache(IConnectionMultiplexer mux)
    {
        _db = mux.GetDatabase();
    }

    private static string BuildField(Guid companyId, Guid roleId)
        => $"{companyId:D}-{roleId:D}";

    /// <inheritdoc />
    public async Task<HashSet<string>?> GetPermissionsAsync(
        Guid companyId,
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        var field = BuildField(companyId, roleId);
        var value = await _db.HashGetAsync(HashKey, field).ConfigureAwait(false);
        if (value.IsNullOrEmpty) return null;

        var list = JsonSerializer.Deserialize<string[]>(value!, JsonOpts);
        if (list is null || list.Length == 0)
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        return list
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public async Task SetPermissionsAsync(
        Guid companyId,
        Guid roleId,
        HashSet<string> permissions,
        CancellationToken cancellationToken = default)
    {
        var field = BuildField(companyId, roleId);

        var normalized = (permissions ?? new HashSet<string>())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var json = JsonSerializer.Serialize(normalized, JsonOpts);
        await _db.HashSetAsync(HashKey, field, json).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task DeletePermissionsAsync(
        Guid companyId,
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        var field = BuildField(companyId, roleId);
        return _db.HashDeleteAsync(HashKey, field);
    }
}

