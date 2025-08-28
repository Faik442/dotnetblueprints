using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.SharedKernel.Security;

/// <summary>
/// Abstraction for role → permissions cache backed by Redis.
/// In this variant, all entries are stored under a single Redis Hash named "Permissions".
/// Field key = "{companyId}-{roleId}", value = JSON array of permission codes.
/// </summary>
public interface IRolePermissionCache
{
    /// <summary>
    /// Gets the permission set for the given company and role. Returns null on cache miss.
    /// </summary>
    Task<HashSet<string>?> GetPermissionsAsync(
        Guid companyId,
        Guid roleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets (overwrites) the permission set for the given company and role.
    /// </summary>
    Task SetPermissionsAsync(
        Guid companyId,
        Guid roleId,
        HashSet<string> permissions,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the cache entry for the given company and role (HDEL on the hash).
    /// </summary>
    Task DeletePermissionsAsync(
        Guid companyId,
        Guid roleId,
        CancellationToken cancellationToken = default);
}

