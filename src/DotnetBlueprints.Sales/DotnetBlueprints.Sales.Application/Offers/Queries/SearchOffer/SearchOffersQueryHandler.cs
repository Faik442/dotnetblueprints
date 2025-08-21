using DotnetBlueprints.Elastic.Services;
using DotnetBlueprints.Sales.Domain.Entities;
using MediatR;
using Nest;

namespace DotnetBlueprints.Sales.Application.Offers.Queries.SearchOffer;

/// <summary>
/// Handles <see cref="SearchOffersQuery"/> by executing a search on ElasticSearch.
/// </summary>
public class SearchOffersQueryHandler : IRequestHandler<SearchOffersQuery, ISearchResponse<Offer>>
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
    public async Task<ISearchResponse<Offer>> Handle(SearchOffersQuery request, CancellationToken cancellationToken)
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

        return response;
    }
}
