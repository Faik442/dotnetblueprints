using DotnetBlueprints.Auth.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DotnetBlueprints.Auth.Application.Features.User.Commands.RemoveUserFromCompany;

public sealed record RemoveUserFromCompanyCommand(Guid UserId, Guid CompanyId) : IRequest;

public sealed class RemoveUserFromCompanyCommandHandler : IRequestHandler<RemoveUserFromCompanyCommand>
{
    private readonly IAuthDbContext _db;
    public RemoveUserFromCompanyCommandHandler(IAuthDbContext db) => _db = db;

    public async Task Handle(RemoveUserFromCompanyCommand req, CancellationToken ct)
    {
        var membership = await _db.Users
            .FirstOrDefaultAsync(uc => uc.Id == req.UserId && uc.CompanyId == req.CompanyId && !uc.IsDeleted, ct)
            ?? throw new KeyNotFoundException("Membership not found.");

        _db.Users.Remove(membership);

        await _db.SaveChangesAsync(ct);
    }
}
