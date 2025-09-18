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
        var membership = await _db.Users
            .Include(uc => uc.UserRoles)
            .FirstOrDefaultAsync(uc => uc.Id == req.UserId, ct)
            ?? throw new KeyNotFoundException("Membership not found.");

        membership.DeleteRoles(req.RoleIds);
        await _db.SaveChangesAsync(ct);
    }
}

