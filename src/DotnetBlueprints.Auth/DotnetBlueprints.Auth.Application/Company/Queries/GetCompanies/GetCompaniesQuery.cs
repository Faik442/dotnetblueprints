using AutoMapper;
using DotnetBlueprints.Auth.Application.Common.Interfaces;
using DotnetBlueprints.Auth.Application.Company.Queries.GetCompanyById;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Company.Queries.GetCompanies;

/// <summary>
/// Query to list all companies.
/// </summary>
public sealed record GetCompaniesQuery() : IRequest<IReadOnlyList<CompanyDto>>;

/// <summary>
/// Handles listing all companies.
/// </summary>
public sealed class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQuery, IReadOnlyList<CompanyDto>>
{
    private readonly IAuthDbContext _context;
    private readonly IMapper _mapper;

    public GetCompaniesQueryHandler(IAuthDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CompanyDto>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        var companies = await _context.Companies
            .Include(c => c.Roles)
            .Include(c => c.Roles)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<CompanyDto>>(companies);
    }
}

