using DotnetBlueprints.Auth.Application.Interfaces;
using DotnetBlueprints.SharedKernel.Exceptions;
using DotnetBlueprints.SharedKernel.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.Company.Commands.UpdateCompanyRole;

/// <summary>
/// Updates a company's role with new name/description.
/// </summary>
public sealed record UpdateCompanyRoleCommand : IRequest<Unit>
{
    /// <summary>Owning company identifier of the role.</summary>
    public Guid CompanyId { get; init; }

    /// <summary>Identifier of the role to update.</summary>
    public Guid RoleId { get; init; }

    /// <summary>New display name for the role (must be unique within the company).</summary>
    public string Name { get; init; } = default!;
}

/// <summary>
/// Handles UpdateCompanyRoleCommand by applying changes and enforcing concurrency.
/// </summary>
public sealed class UpdateCompanyRoleCommandHandler : IRequestHandler<UpdateCompanyRoleCommand, Unit>
{
    private readonly IAuthDbContext _context;
    private readonly IRolePermissionCache _permissionsCache;

    public UpdateCompanyRoleCommandHandler(IAuthDbContext context, IRolePermissionCache permissionsCache)
    {
        _context = context;
        _permissionsCache = permissionsCache;
    }

    public async Task<Unit> Handle(UpdateCompanyRoleCommand request, CancellationToken ct)
    {
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == request.RoleId && r.CompanyId == request.CompanyId, ct);

        if (role is null)
            throw new NotFoundException(nameof(Domain.Entities.Role), request.RoleId);

        role.Rename(request.Name);

        await _context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
