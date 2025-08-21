using DotnetBlueprints.Auth.Application.Common.Interfaces;
using MediatR;

namespace DotnetBlueprints.Auth.Application.Company.Commands.CreateCompany;

/// <summary>
/// Command to create a new company.
/// </summary>
public sealed record CreateCompanyCommand(string Name, string CreatedBy) : IRequest<Guid>;

/// <summary>
/// Handles company creation.
/// </summary>
public sealed class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Guid>
{
    private readonly IAuthDbContext _context;

    public CreateCompanyCommandHandler(IAuthDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = new Domain.Entities.Company(request.Name);

        _context.Companies.Add(company);
        await _context.SaveChangesAsync(cancellationToken);

        return company.Id;
    }
}

