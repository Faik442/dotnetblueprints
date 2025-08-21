using DotnetBlueprints.Sales.Domain.Entities;
using DotnetBlueprints.SharedKernel.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DotnetBlueprints.Sales.Application.Common.Interfaces;

/// <summary>
/// Abstraction for the sales module's database context. 
/// Provides access to entities and supports dependency injection
/// without exposing EF Core directly in the application layer.
/// </summary>
public interface ISalesDbContext
{
    DbSet<Offer> Offers { get; }
    DbSet<Domain.Entities.OfferItem> OfferItems { get; }
    DbSet<OutboxMessage> OutboxMessages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
