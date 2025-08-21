using DotnetBlueprints.Elastic.Clients;
using Nest;

namespace DotnetBlueprints.Elastic.Services;

public interface IElasticService
{
    Task IndexAsync<T>(T document) where T : class;
    Task<ISearchResponse<T>> SearchAsync<T>(Func<SearchDescriptor<T>, ISearchRequest> selector) where T : class;
}