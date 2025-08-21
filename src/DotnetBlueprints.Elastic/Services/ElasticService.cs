using DotnetBlueprints.Elastic.Clients;
using Nest;

namespace DotnetBlueprints.Elastic.Services;

public class ElasticService : IElasticService
{
    private readonly IElasticClientProvider _clientProvider;

    public ElasticService(IElasticClientProvider clientProvider)
    {
        _clientProvider = clientProvider;
    }

    public async Task IndexAsync<T>(T document) where T : class
    {
        await _clientProvider.Client.IndexDocumentAsync(document);
    }

    public async Task<ISearchResponse<T>> SearchAsync<T>(Func<SearchDescriptor<T>, ISearchRequest> selector) where T : class
    {
        return await _clientProvider.Client.SearchAsync(selector);
    }
}

