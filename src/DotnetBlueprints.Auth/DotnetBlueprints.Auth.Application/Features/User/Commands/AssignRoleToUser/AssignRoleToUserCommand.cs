using DotnetBlueprints.Auth.Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DotnetBlueprints.Auth.Application.Features.User.Commands.AssignRoleToUserInCompany;

/// <summary>Assigns a role to a user within a company membership.</summary>
public sealed record AssignRoleToUserCommand(Guid UserId, List<Guid> RoleIds) : IRequest;

public sealed class AssignRoleToUserInCompanyCommandHandler : IRequestHandler<AssignRoleToUserCommand>
{
    private readonly IAuthDbContext _db;
    public AssignRoleToUserInCompanyCommandHandler(IAuthDbContext db) => _db = db;

    public async Task Handle(AssignRoleToUserCommand req, CancellationToken ct)
    {
        var membership = await _db.Users
            .Include(uc => uc.UserRoles)
            .FirstOrDefaultAsync(uc => uc.Id == req.UserId, ct)
            ?? throw new KeyNotFoundException("Membership not found.");

        var roles = _db.Roles.Where(x => req.RoleIds.Contains(x.Id)).ToList();

        membership.AddRoles(roles);
        await _db.SaveChangesAsync(ct);
    }
}

