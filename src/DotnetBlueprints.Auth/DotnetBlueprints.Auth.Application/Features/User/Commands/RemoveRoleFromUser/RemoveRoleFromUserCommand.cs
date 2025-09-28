using DotnetBlueprints.Auth.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DotnetBlueprints.Auth.Application.Features.User.Commands.RemoveRoleFromUserInCompany;

/// <summary>Removes a role assignment from a user-company membership.</summary>
public sealed record RemoveRoleFromUserCommand(Guid UserId, List<Guid> RoleIds) : IRequest;

public sealed class RemoveRoleFromUserInCompanyCommandHandler : IRequestHandler<RemoveRoleFromUserCommand>
{
    private readonly IAuthDbContext _db;
    public RemoveRoleFromUserInCompanyCommandHandler(IAuthDbContext db) => _db = db;

    public async Task Handle(RemoveRoleFromUserCommand req, CancellationToken ct)
    {
        var userRoles = await _db.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == req.UserId && req.RoleIds.Contains(ur.RoleId), ct)
            ?? throw new KeyNotFoundException("Membership not found.");

        _db.UserRoles.RemoveRange(userRoles);
        await _db.SaveChangesAsync(ct);
    }
}

