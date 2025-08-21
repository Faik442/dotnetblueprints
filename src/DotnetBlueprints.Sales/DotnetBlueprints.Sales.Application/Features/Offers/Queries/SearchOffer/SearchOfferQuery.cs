using DotnetBlueprints.Elastic.Services;
using DotnetBlueprints.Sales.Domain.Entities;
using DotnetBlueprints.Sales.Domain.Enums;
using MediatR;
using Nest;

namespace DotnetBlueprints.Sales.Application.Features.Offers.Queries.SearchOffer;

/// <summary>
/// Represents a query for searching offers with various optional filters.
/// </summary>
public class SearchOffersQuery : MediatR.IRequest<ISearchResponse<Offer>>
{
    /// <summary>
    /// Keyword to match the offer title prefix.
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// Minimum total price filter (inclusive).
    /// </summary>
    public decimal? MinTotalPrice { get; set; }

    /// <summary>
    /// Maximum total price filter (inclusive).
    /// </summary>
    public decimal? MaxTotalPrice { get; set; }

    /// <summary>
    /// Optional status filter.
    /// </summary>
    public OfferStatus? Status { get; set; }

    /// <summary>
    /// Optional start date for the valid until filter.
    /// </summary>
    public DateTime? ValidFrom { get; set; }

    /// <summary>
    /// Optional end date for the valid until filter.
    /// </summary>
    public DateTime? ValidTo { get; set; }
}

