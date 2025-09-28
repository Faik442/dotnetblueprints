using DotnetBlueprints.Auth.Application.Interfaces;
using DotnetBlueprints.SharedKernel.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nest;
using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DotnetBlueprints.Auth.Application.Features.Role.Commands.SetRolePermission;

/// <summary>
/// Replaces the permission set of a role with the provided list (idempotent).
/// Missing permissions are added; extra ones are removed.
/// </summary>
public sealed record SetRolePermissionsCommand(
    Guid RoleId,
    IReadOnlyCollection<Guid> PermissionIds
) : MediatR.IRequest<Unit>
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
        var role = _context.Roles.Include(x => x.RolePermissions).Where(x => x.Id == request.RoleId).First();
        if (role is null)
            throw new KeyNotFoundException($"Role : {request.RoleId}");

        var requested = (request.PermissionIds ?? Array.Empty<Guid>()).ToImmutableHashSet();

        if (!request.AllowEmpty && requested.Count == 0)
            throw new ValidationException("PermissionIds cannot be empty (AllowEmpty is false).");

        var perms = await _context.Permissions
            .Where(p => requested.Contains(p.Id))
            .ToListAsync(ct);

        if (perms.Count != request.PermissionIds!.Count)
            throw new ValidationException($"Some permissions were not found");

        var currentIds = role.RolePermissions.Select(rp => rp.PermissionId).ToImmutableHashSet();

        var toAdd = requested.Except(currentIds).ToArray();
        var toRemove = currentIds.Except(requested).ToArray();

        if (toAdd.Length > 0)
        {
            role.AddPermissions(toAdd);
        }

        if (toRemove.Length > 0)
        {
            role.RemovePermissions(toRemove);
        }

        await _context.SaveChangesAsync(ct);

        var finalCodes = perms
            .Select(p => p.Key).ToList();

        var finalSet = finalCodes
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        await _cache.SetPermissionsAsync(role.Id, finalSet, ct);

        return Unit.Value;
    }
}
