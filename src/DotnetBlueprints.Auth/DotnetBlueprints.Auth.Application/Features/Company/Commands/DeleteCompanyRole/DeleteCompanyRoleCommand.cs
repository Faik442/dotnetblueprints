using DotnetBlueprints.Auth.Application.Interfaces;
using DotnetBlueprints.SharedKernel.Exceptions;
using DotnetBlueprints.SharedKernel.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.Company.Commands.DeleteCompanyRole;

/// <summary>
/// Command to soft-delete a role from a company.
/// </summary>
public sealed record DeleteCompanyRoleCommand(Guid CompanyId, Guid RoleId) : IRequest;

/// <summary>
/// Handles soft deletion of a company's role, hard deletion of rolePermission, userCompanyRules and clears related permission cache.
/// </summary>
public sealed class DeleteCompanyRoleCommandHandler : IRequestHandler<DeleteCompanyRoleCommand>
{
    private readonly IAuthDbContext _context;
    private readonly IRolePermissionCache _cache;

    public DeleteCompanyRoleCommandHandler(IAuthDbContext context, IRolePermissionCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task Handle(DeleteCompanyRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == request.RoleId && r.CompanyId == request.CompanyId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Role), request.RoleId);

        role.Delete();

        await _context.RolePermissions.Where(x => x.RoleId == role.Id).ExecuteDeleteAsync(cancellationToken);
        await _context.UserCompanyRoles.Where(x => x.RoleId == role.Id).ExecuteDeleteAsync(cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        await _cache.DeletePermissionsAsync(request.CompanyId, request.RoleId, cancellationToken);
    }
}
