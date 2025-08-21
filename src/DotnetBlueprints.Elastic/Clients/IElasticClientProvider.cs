using DotnetBlueprints.Elastic.Configuration;
using Microsoft.Extensions.Options;
using Nest;

namespace DotnetBlueprints.Elastic.Clients;

public interface IElasticClientProvider
{
    IElasticClient Client { get; }
}
