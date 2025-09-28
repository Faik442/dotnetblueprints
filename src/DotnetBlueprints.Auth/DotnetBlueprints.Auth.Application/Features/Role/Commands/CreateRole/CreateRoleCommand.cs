using DotnetBlueprints.Auth.Application.Interfaces;
using DotnetBlueprints.Auth.Domain.Entities;
using DotnetBlueprints.Auth.Domain.Enums;
using DotnetBlueprints.SharedKernel.Exceptions;
using DotnetBlueprints.SharedKernel.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.Role.Commands.CreateRole;

/// <summary>
/// Command to add a role with permissions to a company.
/// </summary>
public sealed record CreateRoleCommand(
    string RoleName,
    IEnumerable<Guid> Permissions
) : IRequest<Guid>;

/// <summary>
/// Handles "create role for a company" use case. Persists the role first, then populates the role-permission cache.
/// This order guarantees cache consistency (no cache without a DB commit).
/// </summary>
public sealed class CreateCompanyRoleCommandHandler : IRequestHandler<CreateRoleCommand, Guid>
{
    private readonly IAuthDbContext _context;

    private readonly IRolePermissionCache _cache;

    public CreateCompanyRoleCommandHandler(IAuthDbContext context, IRolePermissionCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Guid> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var perms = await _context.Permissions.Where(x => request.Permissions.Contains(x.Id)).ToListAsync();

        if (perms is null)
        {
            throw new ValidationException("Permissions not matched with db.");
        }

        var role = new Domain.Entities.Role(request.RoleName, request.Permissions);
        _context.Roles.Add(role);
        await _context.SaveChangesAsync(cancellationToken);

        var finalSet = perms
            .Select(x => x.Key)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        await _cache.SetPermissionsAsync(role.Id, finalSet, cancellationToken);

        return role.Id;
    }
}

