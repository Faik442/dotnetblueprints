using DotnetBlueprints.Elastic.Services;
using DotnetBlueprints.Sales.Domain.Entities;
using DotnetBlueprints.Sales.Domain.Enums;
using MediatR;
using Nest;

namespace DotnetBlueprints.Sales.Application.Features.Offers.Queries.SearchOffer;

/// <summary>
/// Represents a query for searching offers with various optional filters.
/// </summary>
public class SearchOffersQuery : MediatR.IRequest<List<Offer>>
{
    /// <summary>
    /// Keyword to match the offer title prefix.
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// CompanyID to match the companies.
    /// </summary>
    public Guid? CompanyId { get; set; }

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

/// <summary>
/// Handles <see cref="SearchOffersQuery"/> by executing a search on ElasticSearch.
/// </summary>
public class SearchOffersQueryHandler : IRequestHandler<SearchOffersQuery, List<Offer>>
{
    private readonly IElasticService _elasticService;

    public SearchOffersQueryHandler(IElasticService elasticService)
    {
        _elasticService = elasticService;
    }

    /// <summary>
    /// Executes the search query against ElasticSearch with the specified filters.
    /// </summary>
    /// <param name="request">The search query parameters.</param>
    /// <param name="cancellationToken">Token for cancelling the operation.</param>
    /// <returns>Search results matching the criteria.</returns>
    public async Task<List<Offer>> Handle(SearchOffersQuery request, CancellationToken cancellationToken)
    {
        var mustQueries = new List<Func<QueryContainerDescriptor<Offer>, QueryContainer>>();

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            mustQueries.Add(q =>
                q.Prefix(p => p
                    .Field(f => f.Title.Suffix("keyword"))
                    .Value(request.Keyword.ToLower())
                )
            );
        }

        if (!string.IsNullOrWhiteSpace(request.CompanyId.ToString()))
        {
            mustQueries.Add(q =>
                q.Match(m => m
                    .Field(f => f.CompanyId)
                    .Query(request.Status.ToString())
                )
            );
        }

        if (request.MinTotalPrice.HasValue || request.MaxTotalPrice.HasValue)
        {
            mustQueries.Add(q =>
                q.Range(r => r
                    .Field(f => f.TotalPrice)
                    .GreaterThanOrEquals(Convert.ToDouble(request.MinTotalPrice))
                    .LessThanOrEquals(Convert.ToDouble(request.MaxTotalPrice))
                )
            );
        }

        if (request.Status.HasValue)
        {
            mustQueries.Add(q =>
                q.Match(m => m
                    .Field(f => f.Status)
                    .Query(request.Status.ToString())
                )
            );
        }

        if (request.ValidFrom.HasValue || request.ValidTo.HasValue)
        {
            mustQueries.Add(q =>
                q.DateRange(dr => dr
                    .Field(f => f.ValidUntil)
                    .GreaterThanOrEquals(request.ValidFrom ?? DateTime.MinValue)
                    .LessThanOrEquals(request.ValidTo ?? DateTime.MaxValue)
                )
            );
        }

        var response = await _elasticService.SearchAsync<Offer>(s => s
            .Query(q => q
                .Bool(b => b
                    .Must(mustQueries)
                )
            )
        );

        return response.Documents.OrderByDescending(x => x.CreatedDate).ToList();
    }
}

