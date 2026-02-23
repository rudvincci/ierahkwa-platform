using Mamey.Government.Modules.Certificates.Core.Domain.Repositories;
using Mamey.Government.Modules.Certificates.Core.EF.Repositories;
using Mamey.Government.Modules.Certificates.Core.Mongo.Repositories;
using Mamey.Government.Modules.Certificates.Core.Redis.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Certificates.Core.Composite;

internal static class Extensions
{
    public static IServiceCollection AddCompositeRepositories(this IServiceCollection services)
    {
        services.AddScoped<CertificatePostgresRepository>();
        services.AddScoped<CertificateMongoRepository>();
        services.AddScoped<CertificateRedisRepository>();
        services.AddScoped<ICertificateRepository, CompositeCertificateRepository>();
        
        return services;
    }
}
