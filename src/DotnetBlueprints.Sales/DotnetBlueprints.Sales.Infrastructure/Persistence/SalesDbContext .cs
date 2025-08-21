using DotnetBlueprints.Sales.Application.Common.Interfaces;
using DotnetBlueprints.Sales.Domain.Entities;
using DotnetBlueprints.SharedKernel.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DotnetBlueprints.Sales.Infrastructure.Persistence;

public class SalesDbContext : DbContext, ISalesDbContext
{

    /// <summary>
    /// Entity Framework Core database context for the Sales module.
    /// Implements <see cref="ISalesDbContext"/> to expose sets for offers and their status history.
    /// </summary>
    public SalesDbContext(DbContextOptions<SalesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Offer> Offers { get; set; }

    public virtual DbSet<OfferItem> OfferItems { get; set; }

    public virtual DbSet<AuditHistory> AuditHistories { get; set; }

    public virtual DbSet<OutboxMessage> OutboxMessages { get; set; }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => Database.BeginTransactionAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Offer>().HasQueryFilter(o => !o.IsDeleted);
    }
}
