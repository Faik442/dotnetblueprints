using DotnetBlueprints.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotnetBlueprints.Auth.Application.Common.Interfaces;

public interface IAuthDbContext
{
    DbSet<AccessToken> AccessTokens { get; }
    DbSet<Domain.Entities.Company> Companies { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Role> Roles { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<User> Users { get; }
    DbSet<UserCompany> UserCompanies { get; }
    DbSet<UserCompanyRole> UserCompanyRoles { get; }
    DbSet<UserPermissionOverride> UserPermissionOverride { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
