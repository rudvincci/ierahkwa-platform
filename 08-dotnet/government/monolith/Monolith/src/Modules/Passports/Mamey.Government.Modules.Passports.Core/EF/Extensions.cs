using Mamey.Government.Modules.Passports.Core.Domain.Repositories;
using Mamey.Government.Modules.Passports.Core.EF.Repositories;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Passports.Core.EF;

public static class Extensions
{
    public static IServiceCollection AddPostgresDb(this IServiceCollection services)
    {
        services
            .AddPostgres<PassportsDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(PassportsDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_Passports", "passports");
            })
            .AddUnitOfWork<PassportsUnitOfWork>();
        services.AddScoped<PassportsUnitOfWork>();
        services.AddTransient<PassportsInitializer>();
        services.AddScoped<PassportPostgresRepository>();

        services.AddInitializer<PassportsInitializer>();
        return services;
    }
}
