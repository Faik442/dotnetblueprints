using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.SharedKernel.Abstractions;

/// <summary>
/// Provides access to the current authenticated user information
/// required for auditing (CreatedBy/UpdatedBy/DeletedBy).
/// </summary>
public interface IUserContext
{
    /// <summary>Gets the stable user identifier (e.g., subject claim).</summary>
    string? UserId { get; }

    /// <summary>Gets a human-readable user name (e.g., email or username).</summary>
    string? UserName { get; }
}

