using DotnetBlueprints.Auth.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.Permission.Query.GetAllPermissions;

/// <summary>
/// Returns all permissions from the database (read-only).
/// </summary>
public sealed record GetAllPermissionsQuery() : IRequest<IReadOnlyCollection<PermissionDto>>;

/// <summary>
/// Reads the permission catalog from DB. No mutations (no CRUD).
/// </summary>
public sealed class GetAllPermissionsQueryHandler
    : IRequestHandler<GetAllPermissionsQuery, IReadOnlyCollection<PermissionDto>>
{
    private readonly IAuthDbContext _context;

    public GetAllPermissionsQueryHandler(IAuthDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<PermissionDto>> Handle(
        GetAllPermissionsQuery request,
        CancellationToken ct)
    {
        var items = await _context.Permissions
            .AsNoTracking()
            .OrderBy(p => p.Key)
            .Select(p => new PermissionDto(
                p.Id,
                p.Key,
                p.Description ?? p.Key
            ))
            .ToListAsync(ct);

        return items;
    }
}
