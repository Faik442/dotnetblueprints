using DotnetBlueprints.Auth.Application.Interfaces;
using DotnetBlueprints.Auth.Domain.Entities;
using DotnetBlueprints.SharedKernel.Exceptions;
using DotnetBlueprints.SharedKernel.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.Role.AssignPermissionToRole;

/// <summary>
/// Assigns one or more permission codes to a role within a company.
/// </summary>
public sealed record AssignPermissionToRoleCommand(
    Guid CompanyId,
    Guid RoleId,
    IEnumerable<Guid> Permissions
) : IRequest<Unit>;

public sealed class AssignPermissionToRoleCommandHandler
    : IRequestHandler<AssignPermissionToRoleCommand, Unit>
{
    private readonly IAuthDbContext _context;
    private readonly IRolePermissionCache _cache;

    /// <summary>
    /// Assigns new permissions to the role. Persists to DB first, then updates the role→permissions cache.
    /// </summary>
    public AssignPermissionToRoleCommandHandler(IAuthDbContext context, IRolePermissionCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Unit> Handle(AssignPermissionToRoleCommand request, CancellationToken ct)
    {
        var company = await _context.Companies
            .Include(c => c.Roles)
                .ThenInclude(r => r.RolePermissions)
            .FirstOrDefaultAsync(c => c.Id == request.CompanyId, ct)
            ?? throw new NotFoundException(nameof(Company), request.CompanyId);

        var role = company.Roles.FirstOrDefault(r => r.Id == request.RoleId)
            ?? throw new NotFoundException(nameof(Role), request.RoleId);

        var requestedIds = (request.Permissions ?? Array.Empty<Guid>()).ToHashSet();

        var validPermissionIds = await _context.Permissions
            .Where(p => requestedIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync(ct);

        var existingIds = role.RolePermissions
            .Select(x => x.PermissionId)
            .ToHashSet();

        foreach (var pid in validPermissionIds)
        {
            if (!existingIds.Contains(pid))
                role.AddPermission(pid);
        }

        await _context.SaveChangesAsync(ct);

        var rolePermIds = role.RolePermissions.Select(rp => rp.PermissionId).ToList();

        var finalCodes = await _context.Permissions
            .Where(p => rolePermIds.Contains(p.Id))
            .Select(p => p.Key)
            .ToListAsync(ct);

        var finalSet = finalCodes
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (finalSet.Count > 0)
            await _cache.SetPermissionsAsync(company.Id, role.Id, finalSet, ct);
        else
            await _cache.DeletePermissionsAsync(company.Id, role.Id, ct);

        return Unit.Value;
    }
}