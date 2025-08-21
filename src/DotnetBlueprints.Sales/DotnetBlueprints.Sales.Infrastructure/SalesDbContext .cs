using DotnetBlueprints.Sales.Application.Common.Interfaces;
using DotnetBlueprints.Sales.Domain.Entities;
using DotnetBlueprints.SharedKernel.Audit;
using Microsoft.EntityFrameworkCore;

namespace DotnetBlueprints.Sales.Infrastructure;

public class SalesDbContext : DbContext, ISalesDbContext
{
    public SalesDbContext(DbContextOptions<SalesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Offer> Offers { get; set; }

    public virtual DbSet<OfferItem> OfferItems { get; set; }
    
    public virtual DbSet<AuditHistory> AuditHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Offer>().HasQueryFilter(o => !o.IsDeleted);
    }
}
