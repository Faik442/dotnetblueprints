using DotnetBlueprints.Auth.Application.Interfaces;
using DotnetBlueprints.Auth.Domain.Entities;
using DotnetBlueprints.SharedKernel.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.Role.SetRolePermission;

/// <summary>
/// Replaces the permission set of a role with the provided list (idempotent).
/// Missing permissions are added; extra ones are removed.
/// </summary>
public sealed record SetRolePermissionsCommand(
    Guid CompanyId,
    Guid RoleId,
    IReadOnlyCollection<Guid> PermissionIds
) : IRequest<Unit>
{
    /// <summary>
    /// When true, an empty list will clear all permissions of the role.
    /// If false and the list is empty, the operation is rejected by the validator.
    /// </summary>
    [DefaultValue(true)]
    public bool AllowEmpty { get; init; } = true;
}

/// <summary>
/// Handles replacing a role's permission set in a company scope.
/// </summary>
public sealed class SetRolePermissionsCommandHandler : IRequestHandler<SetRolePermissionsCommand, Unit>
{
    private readonly IAuthDbContext _context;
    private readonly IRolePermissionCache _cache;

    public SetRolePermissionsCommandHandler(IAuthDbContext context, IRolePermissionCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Unit> Handle(SetRolePermissionsCommand request, CancellationToken ct)
    {
        var company = await _context.Companies
            .Include(c => c.Roles.Where(r => r.Id == request.RoleId))
                .ThenInclude(r => r.RolePermissions)
            .FirstOrDefaultAsync(c => c.Id == request.CompanyId, ct);

        if (company is null)
            throw new KeyNotFoundException($"Company {request.CompanyId} was not found.");

        var role = company.Roles.SingleOrDefault();
        if (role is null)
            throw new KeyNotFoundException($"Role {request.RoleId} does not belong to Company {request.CompanyId}.");

        var requested = (request.PermissionIds ?? Array.Empty<Guid>()).ToImmutableHashSet();

        if (!request.AllowEmpty && requested.Count == 0)
            throw new ValidationException("PermissionIds cannot be empty (AllowEmpty is false).");

        var dbPerms = await _context.Permissions
            .Where(p => requested.Contains(p.Id))
            .ToListAsync(ct);

        var foundIds = dbPerms.Select(p => p.Id).ToImmutableHashSet();
        var missing = requested.Except(foundIds).ToArray();
        if (missing.Length > 0)
            throw new ValidationException($"Some permissions were not found: {string.Join(", ", missing)}");

        var currentIds = role.RolePermissions.Select(rp => rp.PermissionId).ToImmutableHashSet();

        var toAdd = requested.Except(currentIds).ToArray();
        var toRemove = currentIds.Except(requested).ToArray();

        if (toAdd.Length > 0)
        {
            foreach(var perm in toAdd)
            {
                role.AddPermission(perm);
            }
        }

        if (toRemove.Length > 0)
        {
            var linksToRemove = role.RolePermissions
                .Where(rp => toRemove.Contains(rp.PermissionId))
                .ToList();

            _context.RolePermissions.RemoveRange(linksToRemove);
        }

        await _context.SaveChangesAsync(ct);

        var finalCodes = dbPerms
            .Select(p => p.Key).ToList();

        var finalSet = finalCodes
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        await _cache.SetPermissionsAsync(company.Id, role.Id, finalSet, ct);

        return Unit.Value;
    }
}
