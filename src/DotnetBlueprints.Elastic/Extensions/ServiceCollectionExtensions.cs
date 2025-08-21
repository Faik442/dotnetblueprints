using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DotnetBlueprints.Elastic.Clients;
using DotnetBlueprints.Elastic.Services;
using DotnetBlueprints.Elastic.Configuration;

namespace DotnetBlueprints.Elastic.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElasticServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ElasticSettings>(configuration.GetSection("ElasticSettings"));
        services.AddSingleton<IElasticClientProvider, ElasticClientProvider>();
        services.AddScoped<IElasticService, ElasticService>();

        return services;
    }
}
