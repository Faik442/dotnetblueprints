using DotnetBlueprints.Auth.Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.User.Commands.AssignRoleToUserInCompany;

/// <summary>Assigns a role to a user within a company membership.</summary>
public sealed record AssignRoleToUserInCompanyCommand(Guid UserId, Guid CompanyId, Guid RoleId) : IRequest;

public sealed class AssignRoleToUserInCompanyCommandHandler : IRequestHandler<AssignRoleToUserInCompanyCommand>
{
    private readonly IAuthDbContext _db;
    public AssignRoleToUserInCompanyCommandHandler(IAuthDbContext db) => _db = db;

    public async Task Handle(AssignRoleToUserInCompanyCommand req, CancellationToken ct)
    {
        var membership = await _db.Users
            .Include(uc => uc.UserCompanyRoles)
            .FirstOrDefaultAsync(uc => uc.Id == req.UserId && uc.CompanyId == req.CompanyId, ct)
            ?? throw new KeyNotFoundException("Membership not found.");

        var role = await _db.Roles.FindAsync([req.RoleId], ct)
                   ?? throw new KeyNotFoundException("Role not found.");

        membership.AddRole(role);
        await _db.SaveChangesAsync(ct);
    }
}

