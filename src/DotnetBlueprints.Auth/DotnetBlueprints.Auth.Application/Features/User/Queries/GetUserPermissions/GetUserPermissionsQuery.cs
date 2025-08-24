using DotnetBlueprints.Auth.Application.Interfaces;
using DotnetBlueprints.SharedKernel.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Features.User.Queries.GetUserPermissions;

/// <summary>
/// Returns the effective permission keys of the current user within the given company,
/// derived from the user's roles (company-scoped + system-wide).
/// </summary>
public sealed record GetUserPermissionsQuery(Guid CompanyId) : IRequest<IReadOnlyList<string>>;

/// <summary>
/// Computes permission keys from the current user's roles for the specified company.
/// Includes company-scoped and system-wide roles. Distinct result, no pagination.
/// </summary>
public sealed class GetUserPermissionsQueryHandler
    : IRequestHandler<GetUserPermissionsQuery, IReadOnlyList<string>>
{
    private readonly IAuthDbContext _db;
    private readonly ICurrentUser _current;

    public GetUserPermissionsQueryHandler(IAuthDbContext db, ICurrentUser current)
    {
        _db = db;
        _current = current;
    }

    public async Task<IReadOnlyList<string>> Handle(GetUserPermissionsQuery request, CancellationToken ct)
    {
        if (_current.UserId is null)
            throw new UnauthorizedAccessException("Anonymous access.");

        var userId = _current.UserId.Value;
        var companyId = request.CompanyId;

        var userCompanyOk = await _db.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId && !u.IsDeleted && u.CompanyId == companyId, ct);

        if (!userCompanyOk)
            throw new UnauthorizedAccessException("User does not belong to the requested company.");

        var roleIds = await
            (from ucr in _db.UserCompanyRoles.AsNoTracking()
             join r in _db.Roles.AsNoTracking() on ucr.RoleId equals r.Id
             where ucr.UserId == userId
                   && ucr.CompanyId == companyId
                   && !r.IsDeleted
                   && (r.CompanyId == companyId || r.CompanyId == null)
             select r.Id)
            .Distinct()
            .ToListAsync(ct);

        if (roleIds.Count == 0)
            return Array.Empty<string>();

        // RolePermission → Permission.Key (distinct)
        var permKeys = await
            (from rp in _db.RolePermissions.AsNoTracking()
             join p in _db.Permissions.AsNoTracking() on rp.PermissionId equals p.Id
             where roleIds.Contains(rp.RoleId) && !p.IsDeleted
             select p.Key)
            .Distinct()
            .ToListAsync(ct);

        return permKeys;
    }
}
