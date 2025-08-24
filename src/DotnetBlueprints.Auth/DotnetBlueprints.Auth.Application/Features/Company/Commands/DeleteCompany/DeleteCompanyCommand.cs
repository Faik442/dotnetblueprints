using DotnetBlueprints.Auth.Application.Interfaces;
using DotnetBlueprints.SharedKernel.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.Company.Commands.DeleteCompany;


/// <summary>
/// Command to soft-delete a company.
/// </summary>
public sealed record DeleteCompanyCommand(Guid CompanyId, string DeletedBy) : IRequest;

/// <summary>
/// Handles company deletion.
/// </summary>
public sealed class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand>
{
    private readonly IAuthDbContext _context;

    public DeleteCompanyCommandHandler(IAuthDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Id == request.CompanyId, cancellationToken)
            ?? throw new NotFoundException(nameof(Company), request.CompanyId);

        company.Delete();

        await _context.SaveChangesAsync(cancellationToken);
    }
}

