using DotnetBlueprints.Auth.Application.Interfaces;
using DotnetBlueprints.Auth.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.User.Commands.AddUserToCompany;

/// <summary>Creates (or re-activates) a user-company membership and optionally assigns roles.</summary>
public sealed record AddUserToCompanyCommand(Guid CompanyId, string Email, string DisplayName, string Password, IEnumerable<Guid> RoleIds) : IRequest;

public sealed class AddUserToCompanyCommandHandler : IRequestHandler<AddUserToCompanyCommand>
{
    private readonly IAuthDbContext _db;

    private readonly IPasswordHasher _hasher;
    public AddUserToCompanyCommandHandler(IAuthDbContext db, IPasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }
    public async Task Handle(AddUserToCompanyCommand req, CancellationToken ct)
    {
        var company = await _db.Companies.FindAsync([req.CompanyId], ct)
                      ?? throw new KeyNotFoundException("Company not found.");

        var roles = _db.Roles.Where(x => x.CompanyId == req.CompanyId && req.RoleIds.Contains(x.Id));

        var membership = Domain.Entities.User.Create(req.Email, req.DisplayName, _hasher.Hash(req.Password), roles);

        await _db.SaveChangesAsync(ct);
    }
}

