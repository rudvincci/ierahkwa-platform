using Mamey.Government.Modules.Citizens.Core.Domain.Repositories;
using Mamey.Government.Modules.Citizens.Core.EF.Repositories;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Citizens.Core.EF;

public static class Extensions
{
    public static IServiceCollection AddPostgresDb(this IServiceCollection services)
    {
        services
            .AddPostgres<CitizensDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(CitizensDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_Citizens", "citizens");
            })
            .AddUnitOfWork<CitizensUnitOfWork>();
        services.AddScoped<CitizensUnitOfWork>();
        services.AddTransient<CitizensInitializer>();
        services.AddScoped<CitizenPostgresRepository>();

        services.AddInitializer<CitizensInitializer>();
        return services;
    }
}
