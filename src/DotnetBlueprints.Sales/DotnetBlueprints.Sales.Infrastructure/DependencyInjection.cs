using DotnetBlueprints.Elastic.Clients;
using DotnetBlueprints.Elastic.Configuration;
using DotnetBlueprints.Elastic.Services;
using DotnetBlueprints.Sales.Application.Common.Interfaces;
using DotnetBlueprints.Sales.Infrastructure.Audit;
using DotnetBlueprints.Sales.Infrastructure.Persistence;
using DotnetBlueprints.SharedKernel.Audit;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DotnetBlueprints.Sales.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SalesDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DotnetBlueprints_Sales")));

        services.AddScoped<ISalesDbContext>(provider => provider.GetRequiredService<SalesDbContext>());
        services.AddScoped<IAuditHistoryRepository, AuditHistoryRepository>();
        services.Configure<ElasticSettings>(configuration.GetSection("ElasticSettings"));
        services.AddSingleton<IElasticClientProvider, ElasticClientProvider>();
        services.AddScoped<IElasticService, ElasticService>();

        services.AddMassTransit(x =>
        {
            x.AddConsumers(Assembly.GetExecutingAssembly());

            x.SetKebabCaseEndpointNameFormatter();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });


        return services;
    }
}
