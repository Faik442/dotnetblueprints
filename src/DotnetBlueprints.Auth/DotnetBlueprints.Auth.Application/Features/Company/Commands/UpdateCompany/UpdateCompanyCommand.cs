using DotnetBlueprints.Auth.Application.Interfaces;
using DotnetBlueprints.SharedKernel.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.Company.Commands.UpdateCompany;

/// <summary>
/// Command to rename a company.
/// </summary>
public sealed record UpdateCompanyCommand(Guid CompanyId, string NewName) : IRequest;

/// <summary>
/// Handles company renaming.
/// </summary>
public sealed class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand>
{
    private readonly IAuthDbContext _context;

    public UpdateCompanyCommandHandler(IAuthDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Id == request.CompanyId, cancellationToken)
            ?? throw new NotFoundException(nameof(Company), request.CompanyId);

        company.Rename(request.NewName);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
