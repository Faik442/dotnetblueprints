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

        // === Keys ===
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<Role>().HasKey(r => r.Id);
        modelBuilder.Entity<Permission>().HasKey(p => p.Id);
        modelBuilder.Entity<Company>().HasKey(p => p.Id);
        modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
        modelBuilder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });

        // === Relations ===
        modelBuilder.Entity<User>()
            .HasOne(u => u.Company)
            .WithMany(c => c.Members)
            .HasForeignKey(u => u.CompanyId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId);

        // === Index/constraints ===
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email).IsUnique(); // İstersen (CompanyId, Email) unique de yapabilirsin

        modelBuilder.Entity<Role>()
            .HasIndex(r => r.Name).IsUnique();   // Global benzersiz

        modelBuilder.Entity<Permission>()
            .HasIndex(p => p.Key).IsUnique();

        // === Soft-delete filtreleri (varsa)
        modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Role>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Permission>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Company>().HasQueryFilter(x => !x.IsDeleted);

    }
}
