using DotnetBlueprints.Auth.Application.Common.Interfaces;
using DotnetBlueprints.Auth.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.User.Commands;

/// <summary>Creates (or re-activates) a user-company membership and optionally assigns roles.</summary>
public sealed record AddUserToCompanyCommand(
    Guid UserId, Guid CompanyId, IEnumerable<Guid>? RoleIds, bool IsPrimary = false
) : IRequest;

public sealed class AddUserToCompanyCommandHandler : IRequestHandler<AddUserToCompanyCommand>
{
    private readonly IAuthDbContext _db;
    public AddUserToCompanyCommandHandler(IAuthDbContext db) => _db = db;

    public async Task Handle(AddUserToCompanyCommand req, CancellationToken ct)
    {
        var user = await _db.Users.FindAsync([req.UserId], ct)
                   ?? throw new KeyNotFoundException("User not found.");
        var company = await _db.Companies.FindAsync([req.CompanyId], ct)
                      ?? throw new KeyNotFoundException("Company not found.");

        var membership = await _db.UserCompanies
            .Include(uc => uc.UserCompanyRoles)
            .FirstOrDefaultAsync(uc => uc.UserId == req.UserId && uc.CompanyId == req.CompanyId, ct);

        if (membership is null)
        {
            membership = new UserCompany(req.UserId, req.CompanyId, isPrimary: false);
            _db.UserCompanies.Add(membership);
        }

        // primary handling (only one true — do at User aggregate level ideally)
        if (req.IsPrimary)
        {
            var all = await _db.UserCompanies.Where(uc => uc.UserId == req.UserId).ToListAsync(ct);
            foreach (var uc in all) uc.UnsetPrimary();
            membership.SetPrimary();
        }

        if (req.RoleIds is not null)
        {
            var roles = await _db.Roles.Where(r => req.RoleIds.Contains(r.Id)).ToListAsync(ct);
            foreach (var role in roles)
                membership.AddRole(role);
        }

        await _db.SaveChangesAsync(ct);
    }
}

