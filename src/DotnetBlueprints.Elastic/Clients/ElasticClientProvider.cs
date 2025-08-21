using DotnetBlueprints.Elastic.Configuration;
using Microsoft.Extensions.Options;
using Nest;

namespace DotnetBlueprints.Elastic.Clients;

public class ElasticClientProvider : IElasticClientProvider
{
    public IElasticClient Client { get; }

    public ElasticClientProvider(IOptions<ElasticSettings> options)
    {
        var settings = new ConnectionSettings(new Uri(options.Value.Url))
            .DefaultIndex(options.Value.IndexFormat);

        Client = new ElasticClient(settings);
    }
}