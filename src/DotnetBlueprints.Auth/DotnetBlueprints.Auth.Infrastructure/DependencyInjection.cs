using Microsoft.Extensions.DependencyInjection;
using DotnetBlueprints.Auth.Infrastructure.Persistence;
using DotnetBlueprints.SharedKernel.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using DotnetBlueprints.Auth.Infrastructure.Identity;
using DotnetBlueprints.SharedKernel.Security;

namespace DotnetBlueprints.Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<AuditInterceptor>();

        services.AddDbContext<AuthDbContext>((sp, opt) =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("AuthDb"));
            opt.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
        });

        return services;
    }
}
