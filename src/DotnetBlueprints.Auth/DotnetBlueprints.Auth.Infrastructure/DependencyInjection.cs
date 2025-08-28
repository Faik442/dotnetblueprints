using DotnetBlueprints.Auth.Infrastructure.Identity;
using DotnetBlueprints.Auth.Infrastructure.Persistence;
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
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));

        services.AddScoped<IRolePermissionCache, RedisRolePermissionHashCache>();

        services.AddDbContext<AuthDbContext>((sp, opt) =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("AuthDb"));
            opt.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
        });

        return services;
    }
}
