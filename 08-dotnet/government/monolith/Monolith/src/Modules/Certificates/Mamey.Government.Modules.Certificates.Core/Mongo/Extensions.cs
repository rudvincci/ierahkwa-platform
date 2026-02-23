using Mamey.Government.Modules.Certificates.Core.Mongo.Repositories;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Certificates.Core.Mongo;

public static class Extensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services)
    {
        services.AddMongoRepository<Documents.CertificateDocument, Guid>("certificates.certificates");
        services.AddScoped<CertificateMongoRepository>();
        
        return services;
    }
}
