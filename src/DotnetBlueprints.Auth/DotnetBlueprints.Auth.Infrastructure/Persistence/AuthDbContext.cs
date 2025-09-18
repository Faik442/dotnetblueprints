using DotnetBlueprints.Auth.Application.Interfaces;
using DotnetBlueprints.Auth.Domain.Entities;
using DotnetBlueprints.SharedKernel.Audit;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DotnetBlueprints.Auth.Infrastructure.Persistence;

/// <summary>
/// EF Core database context for authentication and authorization.
/// </summary>
public sealed class AuthDbContext : DbContext, IAuthDbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    // === DbSets ===
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<AccessToken> AccessTokens { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<AuditHistory> AuditHistories { get; set; }

    /// <summary>
    /// Configures model mappings and global query filters (soft-delete).
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // === Composite Keys ===
        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        modelBuilder.Entity<UserRole>()
            .HasKey(ucr => new { ucr.UserId, ucr.RoleId });

        // === Relationships ===
        modelBuilder.Entity<User>()
            .HasKey(uc => new { uc.Id, uc.CompanyId });

        modelBuilder.Entity<User>()
            .HasMany(uc => uc.UserRoles)
            .WithOne(ucr => ucr.User)
            .HasForeignKey(ucr => new { ucr.UserId });

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId);

        // === Token Config ===
        modelBuilder.Entity<RefreshToken>()
            .HasIndex(rt => rt.Hash)
            .IsUnique();

        modelBuilder.Entity<AccessToken>()
            .HasIndex(at => at.JwtId)
            .IsUnique();

        // === String lengths (optional, best practice) ===
        modelBuilder.Entity<User>(e =>
        {
            e.Property(u => u.Email).HasMaxLength(256).IsRequired();
            e.Property(u => u.DisplayName).HasMaxLength(256);
        });

        modelBuilder.Entity<Permission>(e =>
        {
            e.Property(p => p.Key).HasMaxLength(128).IsRequired();
            e.Property(p => p.Description).HasMaxLength(512);
        });

        modelBuilder.Entity<Role>(e =>
        {
            e.Property(r => r.Name).HasMaxLength(128).IsRequired();
        });

        modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Role>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Permission>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Company>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Role>().HasIndex(r => r.Name).IsUnique();
        modelBuilder.Entity<Permission>().HasIndex(p => p.Key).IsUnique();
        modelBuilder.Entity<RefreshToken>().HasIndex(rt => rt.Hash).IsUnique();
    }
}
