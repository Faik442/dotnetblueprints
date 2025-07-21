using DotnetBlueprints.Sales.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotnetBlueprints.Sales.Application.Common.Interfaces;

/// <summary>
/// Abstraction for the sales module's database context. 
/// Provides access to entities and supports dependency injection
/// without exposing EF Core directly in the application layer.
/// </summary>
public interface ISalesDbContext
{
    DbSet<Offer> Offers { get; }
    DbSet<OfferStatusHistory> OfferStatusHistories { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
