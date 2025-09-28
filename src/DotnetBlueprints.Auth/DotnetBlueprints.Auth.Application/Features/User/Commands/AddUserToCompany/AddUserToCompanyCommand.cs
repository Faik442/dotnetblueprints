using DotnetBlueprints.Auth.Application.Interfaces;
using FluentValidation;
using MediatR;

namespace DotnetBlueprints.Auth.Application.Features.User.Commands.AddUserToCompany;

/// <summary>Creates (or re-activates) a user-company membership and optionally assigns roles.</summary>
public sealed record AddUserToCompanyCommand(Guid CompanyId, string Email, string DisplayName, string Password, IEnumerable<Guid> RoleIds) : IRequest<Guid>;

public sealed class AddUserToCompanyCommandHandler : IRequestHandler<AddUserToCompanyCommand, Guid>
{
    private readonly IAuthDbContext _db;

    private readonly IPasswordHasher _hasher;
    public AddUserToCompanyCommandHandler(IAuthDbContext db, IPasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }
    public async Task<Guid> Handle(AddUserToCompanyCommand req, CancellationToken ct)
    {
        var company = await _db.Companies.FindAsync([req.CompanyId], ct)
                      ?? throw new KeyNotFoundException("Company not found.");

        var roles = _db.Roles.Where(x => req.RoleIds.Contains(x.Id));

        var user = Domain.Entities.User.Create(req.CompanyId, req.Email, req.DisplayName, _hasher.Hash(req.Password), roles);
        _db.Users.Add(user);

        await _db.SaveChangesAsync(ct);

        return user.Id;
    }
}

