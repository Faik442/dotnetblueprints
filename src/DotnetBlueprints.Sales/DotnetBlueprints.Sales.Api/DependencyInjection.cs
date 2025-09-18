using DotnetBlueprints.SharedKernel.Security;
using Microsoft.OpenApi.Models;

namespace DotnetBlueprints.Sales.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "DotnetBlueprints.Auth API",
                Version = "v1",
                Description = "Enterprise-grade auth API with modular structure and best practices."
            });
        });

        services.AddControllers(o =>
        {
            o.Filters.Add<PermissionAuthorizeFilter>();
        });

        return services;
    }
}
