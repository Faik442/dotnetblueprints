using DotnetBlueprints.Auth.Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.User.Commands.RemoveRoleFromUserInCompany;

/// <summary>Removes a role assignment from a user-company membership.</summary>
public sealed record RemoveRoleFromUserInCompanyCommand(Guid UserId, Guid CompanyId, Guid RoleId) : IRequest;

public sealed class RemoveRoleFromUserInCompanyCommandHandler : IRequestHandler<RemoveRoleFromUserInCompanyCommand>
{
    private readonly IAuthDbContext _db;
    public RemoveRoleFromUserInCompanyCommandHandler(IAuthDbContext db) => _db = db;

    public async Task Handle(RemoveRoleFromUserInCompanyCommand req, CancellationToken ct)
    {
        var membership = await _db.Users
            .Include(uc => uc.UserCompanyRoles)
            .FirstOrDefaultAsync(uc => uc.Id == req.UserId && uc.CompanyId == req.CompanyId, ct)
            ?? throw new KeyNotFoundException("Membership not found.");

        // domain’de RemoveRole varsa onu çağır
        membership.DeleteRole(req.RoleId);
        await _db.SaveChangesAsync(ct);
    }
}

