using DotnetBlueprints.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotnetBlueprints.Auth.Application.Interfaces;

public interface IAuthDbContext
{
    DbSet<AccessToken> AccessTokens { get; }
    DbSet<Company> Companies { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Role> Roles { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<User> Users { get; }
    DbSet<UserCompanyRole> UserCompanyRoles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
