using Mamey.Government.Modules.TravelIdentities.Core.Domain.Repositories;
using Mamey.Government.Modules.TravelIdentities.Core.EF.Repositories;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.TravelIdentities.Core.EF;

public static class Extensions
{
    public static IServiceCollection AddPostgresDb(this IServiceCollection services)
    {
        services
            .AddPostgres<TravelIdentitiesDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(TravelIdentitiesDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_TravelIdentities", "travel_identities");
            })
            .AddUnitOfWork<TravelIdentitiesUnitOfWork>();
        services.AddScoped<TravelIdentitiesUnitOfWork>();
        services.AddTransient<TravelIdentitiesInitializer>();
        services.AddScoped<TravelIdentityPostgresRepository>();

        services.AddInitializer<TravelIdentitiesInitializer>();
        return services;
    }
}
