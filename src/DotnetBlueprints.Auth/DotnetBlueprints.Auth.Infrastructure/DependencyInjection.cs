using DotnetBlueprints.Auth.Application.Interfaces;
using DotnetBlueprints.Auth.Infrastructure.Audit;
using DotnetBlueprints.Auth.Infrastructure.Identity;
using DotnetBlueprints.Auth.Infrastructure.Persistence;
using DotnetBlueprints.Auth.Infrastructure.Services;
using DotnetBlueprints.Elastic.Configuration;
using DotnetBlueprints.SharedKernel.Audit;
using DotnetBlueprints.SharedKernel.Infrastructure;
using DotnetBlueprints.SharedKernel.Redis;
using DotnetBlueprints.SharedKernel.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DotnetBlueprints.Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<AuditInterceptor>();
        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));

        services.AddScoped<IAuthDbContext>(provider => provider.GetRequiredService<AuthDbContext>());
        services.AddScoped<IAuditHistoryRepository, AuditHistoryRepository>();
        services.Configure<ElasticSettings>(configuration.GetSection("ElasticSettings"));
        services.AddScoped<IRolePermissionCache, RolePermissionCache>();
        services.AddScoped<IPasswordHasher, AspNetPasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddDbContext<AuthDbContext>((sp, options) =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DotnetBlueprints_Auth"),
                sql => sql.MigrationsAssembly(typeof(AuthDbContext).Assembly.GetName().Name));

            options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
        });

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
