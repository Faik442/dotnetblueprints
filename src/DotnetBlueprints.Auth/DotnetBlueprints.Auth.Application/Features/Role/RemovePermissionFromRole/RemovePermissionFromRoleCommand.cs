using DotnetBlueprints.Auth.Application.Interfaces;
using DotnetBlueprints.SharedKernel.Exceptions;
using DotnetBlueprints.SharedKernel.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.Role.RemovePermissionFromRole;

public sealed record RemovePermissionFromRoleCommand(
    Guid CompanyId,
    Guid RoleId,
    IReadOnlyCollection<Guid> Permissions
) : IRequest<Unit>;

public sealed class RemovePermissionFromRoleCommandHandler
    : IRequestHandler<RemovePermissionFromRoleCommand, Unit>
{
    private readonly IAuthDbContext _context;
    private readonly IRolePermissionCache _cache;

    /// <summary>
    /// Removes one or more permissions (by Id) from a role within a company.
    /// Validates against DB, updates the aggregate, persists, then refreshes the cache.
    /// </summary>
    public RemovePermissionFromRoleCommandHandler(IAuthDbContext context, IRolePermissionCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Unit> Handle(RemovePermissionFromRoleCommand request, CancellationToken ct)
    {
        // 1) Load company → role → role-permissions
        var company = await _context.Companies
            .Include(c => c.Roles)
                .ThenInclude(r => r.RolePermissions)
            .FirstOrDefaultAsync(c => c.Id == request.CompanyId, ct)
            ?? throw new NotFoundException(nameof(Company), request.CompanyId);

        var role = company.Roles.FirstOrDefault(r => r.Id == request.RoleId)
            ?? throw new NotFoundException(nameof(Role), request.RoleId);

        var requestedIds = (request.Permissions ?? Array.Empty<Guid>()).ToHashSet();
        if (requestedIds.Count == 0) return Unit.Value;

        var validIds = await _context.Permissions
            .Where(p => requestedIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync(ct);

        if (validIds.Count == 0)
            throw new ValidationException("Permissions not matched with DB.");

        var existingIds = role.RolePermissions
            .Select(rp => rp.PermissionId)
            .ToHashSet();

        var idsToRemove = validIds.Where(id => existingIds.Contains(id)).ToList();
        if (idsToRemove.Count == 0) return Unit.Value;

        var toDelete = role.RolePermissions
            .Where(rp => idsToRemove.Contains(rp.PermissionId))
            .ToList();

        foreach (var rp in toDelete)
            role.RemovePermission(rp.PermissionId);

        await _context.SaveChangesAsync(ct);

        var remainingIds = role.RolePermissions.Select(rp => rp.PermissionId).ToList();

        var finalCodes = remainingIds.Count == 0
            ? new List<string>()
            : await _context.Permissions
                .Where(p => remainingIds.Contains(p.Id))
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