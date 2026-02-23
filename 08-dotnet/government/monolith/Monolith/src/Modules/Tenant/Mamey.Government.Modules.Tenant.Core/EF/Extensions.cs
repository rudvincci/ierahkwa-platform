using Mamey.Government.Modules.Tenant.Core.Domain.Repositories;
using Mamey.Government.Modules.Tenant.Core.EF.Repositories;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Tenant.Core.EF;

public static class Extensions
{
    public static IServiceCollection AddPostgresDb(this IServiceCollection services)
    {
        services
            .AddPostgres<TenantDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(TenantDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_Tenant", "tenant");
            })
            .AddUnitOfWork<TenantUnitOfWork>();
        services.AddScoped<TenantUnitOfWork>();
        services.AddTransient<TenantInitializer>();
        services.AddScoped<TenantPostgresRepository>();

        services.AddInitializer<TenantInitializer>();
        return services;
    }
}
