using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.SharedKernel.Security;

/// <summary>
/// Declarative permission requirement for an endpoint (Controller/Action).
/// You can attach this attribute multiple times; ALL listed permissions must be present.
/// Example:
///   [RequirePermission("dealer.create")]
///   [RequirePermission("dealer.write")]
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class RequirePermissionAttribute : Attribute
{
    /// <summary>
    /// Creates a new permission requirement.
    /// </summary>
    /// <param name="permission">Permission code, e.g., "dealer.create".</param>
    public RequirePermissionAttribute(string permission)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permission);
        Permission = permission;
    }

    /// <summary>
    /// Permission code (e.g., "dealer.create").
    /// </summary>
    public string Permission { get; }
}