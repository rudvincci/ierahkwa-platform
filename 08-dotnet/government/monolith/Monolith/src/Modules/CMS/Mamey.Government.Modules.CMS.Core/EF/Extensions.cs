using Mamey.Government.Modules.CMS.Core.Domain.Repositories;
using Mamey.Government.Modules.CMS.Core.EF.Repositories;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.CMS.Core.EF;

public static class Extensions
{
    public static IServiceCollection AddPostgresDb(this IServiceCollection services)
    {
        services
            .AddPostgres<CMSDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(CMSDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_CMS", "cms");
            })
            .AddUnitOfWork<CMSUnitOfWork>();
        services.AddScoped<CMSUnitOfWork>();
        services.AddTransient<CMSInitializer>();
        services.AddScoped<ContentPostgresRepository>();

        services.AddInitializer<CMSInitializer>();
        return services;
    }
}
