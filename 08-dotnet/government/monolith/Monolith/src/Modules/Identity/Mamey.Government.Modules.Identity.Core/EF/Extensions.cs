using Mamey.Government.Modules.Identity.Core.Domain.Repositories;
using Mamey.Government.Modules.Identity.Core.EF.Repositories;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Identity.Core.EF;

public static class Extensions
{
    public static IServiceCollection AddPostgresDb(this IServiceCollection services)
    {
        services
            .AddPostgres<IdentityDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_Identity", "identity");
            })
            .AddUnitOfWork<IdentityUnitOfWork>();
        services.AddScoped<IdentityUnitOfWork>();
        services.AddTransient<IdentityInitializer>();
        services.AddScoped<UserProfilePostgresRepository>();

        services.AddInitializer<IdentityInitializer>();
        return services;
    }
}
