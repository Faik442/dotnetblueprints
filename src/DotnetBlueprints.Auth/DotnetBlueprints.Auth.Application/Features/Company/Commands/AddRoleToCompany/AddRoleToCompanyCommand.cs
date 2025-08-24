using DotnetBlueprints.Auth.Application.Interfaces;
using DotnetBlueprints.Auth.Domain.Entities;
using DotnetBlueprints.Auth.Domain.Enums;
using DotnetBlueprints.SharedKernel.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.Company.Commands.AddRoleToCompany;

/// <summary>
/// Command to add a role with permissions to a company.
/// </summary>
public sealed record AddRoleToCompanyCommand(
    Guid CompanyId,
    string RoleName,
    IEnumerable<PermissionKey> Permissions
) : IRequest<Guid>;

/// <summary>
/// Handles adding a role to a company.
/// </summary>
public sealed class AddRoleToCompanyCommandHandler : IRequestHandler<AddRoleToCompanyCommand, Guid>
{
    private readonly IAuthDbContext _context;

    public AddRoleToCompanyCommandHandler(IAuthDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(AddRoleToCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies
            .Include(c => c.Roles)
            .ThenInclude(r => r.RolePermissions)
            .FirstOrDefaultAsync(c => c.Id == request.CompanyId, cancellationToken)
            ?? throw new NotFoundException(nameof(Company), request.CompanyId);

        var role = company.AddRole(request.RoleName, request.Permissions);

        await _context.SaveChangesAsync(cancellationToken);

        return role.Id;
    }
}

