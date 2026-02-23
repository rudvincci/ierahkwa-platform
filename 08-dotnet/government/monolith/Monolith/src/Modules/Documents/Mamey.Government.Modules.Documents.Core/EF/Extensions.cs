using Mamey.Government.Modules.Documents.Core.Domain.Repositories;
using Mamey.Government.Modules.Documents.Core.EF.Repositories;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Documents.Core.EF;

public static class Extensions
{
    public static IServiceCollection AddPostgresDb(this IServiceCollection services)
    {
        services
            .AddPostgres<DocumentsDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(DocumentsDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_Documents", "documents");
            })
            .AddUnitOfWork<DocumentsUnitOfWork>();
        services.AddScoped<DocumentsUnitOfWork>();
        services.AddTransient<DocumentsInitializer>();
        services.AddScoped<DocumentPostgresRepository>();

        services.AddInitializer<DocumentsInitializer>();
        return services;
    }
}
