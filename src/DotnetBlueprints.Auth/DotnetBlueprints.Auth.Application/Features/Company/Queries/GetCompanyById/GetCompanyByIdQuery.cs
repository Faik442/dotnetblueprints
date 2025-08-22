using AutoMapper;
using DotnetBlueprints.Auth.Application.Common.Interfaces;
using DotnetBlueprints.SharedKernel.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.Company.Queries.GetCompanyById;

/// <summary>
/// Query to get a company by Id.
/// </summary>
public sealed record GetCompanyByIdQuery(Guid CompanyId) : IRequest<CompanyDto>;

/// <summary>
/// Handles company retrieval by Id.
/// </summary>
public sealed class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, CompanyDto>
{
    private readonly IAuthDbContext _context;
    private readonly IMapper _mapper;

    public GetCompanyByIdQueryHandler(IAuthDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CompanyDto> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies
            .Include(c => c.Roles)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(c => c.Id == request.CompanyId, cancellationToken)
            ?? throw new NotFoundException(nameof(Company), request.CompanyId);

        return _mapper.Map<CompanyDto>(company);
    }
}
