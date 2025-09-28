using AutoMapper;
using AutoMapper.QueryableExtensions;
using DotnetBlueprints.Auth.Application.Features.Company.Queries.GetCompanyById;
using DotnetBlueprints.Auth.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DotnetBlueprints.Auth.Application.Features.Company.Queries.GetCompanies;

/// <summary>
/// Query to list all companies.
/// </summary>
public sealed record GetCompaniesQuery(string? companyName,
    int page = 1,
    int pageSize = 20) : IRequest<IReadOnlyList<Domain.Entities.Company>>;

/// <summary>
/// Handles listing all companies.
/// </summary>
public sealed class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQuery, IReadOnlyList<Domain.Entities.Company>>
{
    private readonly IAuthDbContext _context;
    private readonly IMapper _mapper;
    private const int MaxPageSize = 100;

    public GetCompaniesQueryHandler(IAuthDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<Domain.Entities.Company>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        var size = Math.Clamp(request.pageSize, 1, MaxPageSize);
        var skip = Math.Max(request.page, 1);
        skip = (skip - 1) * size;

        var q = _context.Companies.Include(x => x.Members).ThenInclude(x => x.UserRoles).AsNoTracking();

        q = string.IsNullOrWhiteSpace(request.companyName)
            ? q
            : q.Where(c => EF.Functions.Like(c.Name, $"%{request.companyName.Trim()}%"));

        return await q
            .OrderBy(c => c.Name)
            .Skip(skip)
            .Take(size)
            .ToListAsync(cancellationToken);
    }
}

