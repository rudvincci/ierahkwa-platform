using Mamey.Government.Modules.Documents.Core.Domain.Repositories;
using Mamey.Government.Modules.Documents.Core.EF.Repositories;
using Mamey.Government.Modules.Documents.Core.Mongo.Repositories;
using Mamey.Government.Modules.Documents.Core.Redis.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Documents.Core.Composite;

internal static class Extensions
{
    public static IServiceCollection AddCompositeRepositories(this IServiceCollection services)
    {
        services.AddScoped<DocumentPostgresRepository>();
        services.AddScoped<DocumentMongoRepository>();
        services.AddScoped<DocumentRedisRepository>();
        services.AddScoped<IDocumentRepository, CompositeDocumentRepository>();
        
        return services;
    }
}
