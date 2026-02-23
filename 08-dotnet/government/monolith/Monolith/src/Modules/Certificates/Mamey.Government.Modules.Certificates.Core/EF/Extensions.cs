using Mamey.Government.Modules.Certificates.Core.Domain.Repositories;
using Mamey.Government.Modules.Certificates.Core.EF.Repositories;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Certificates.Core.EF;

public static class Extensions
{
    public static IServiceCollection AddPostgresDb(this IServiceCollection services)
    {
        services
            .AddPostgres<CertificatesDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(CertificatesDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_Certificates", "certificates");
            })
            .AddUnitOfWork<CertificatesUnitOfWork>();
        services.AddScoped<CertificatesUnitOfWork>();
        services.AddTransient<CertificatesInitializer>();
        services.AddScoped<CertificatePostgresRepository>();

        services.AddInitializer<CertificatesInitializer>();
        return services;
    }
}
