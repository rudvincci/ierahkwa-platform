using Mamey.Government.Modules.CitizenshipApplications.Core.EF.Repositories;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.EF;

public static class Extensions
{
    public static IServiceCollection AddPostgresDb(this IServiceCollection services)
    {
            
        services
            .AddPostgres<CitizenshipApplicationsDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(CitizenshipApplicationsDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_CitizenshipApplications", "citizenship_applications");
            })
            .AddUnitOfWork<CitizenshipApplicationsUnitOfWork>();
        services.AddScoped<CitizenshipApplicationsUnitOfWork>();
        services.AddTransient<CitizenshipApplicationsInitializer>();
        services.AddScoped<ApplicationPostgresRepository>();
        services.AddScoped<EF.Repositories.UploadedDocumentPostgresRepository>();
        services.AddScoped<Domain.Repositories.IApplicationTokenRepository, EF.Repositories.ApplicationTokenPostgresRepository>();

        services.AddInitializer<CitizenshipApplicationsInitializer>();
        return services;
    }
}
