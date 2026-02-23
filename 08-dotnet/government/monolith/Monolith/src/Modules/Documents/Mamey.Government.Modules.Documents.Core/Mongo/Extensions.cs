using Mamey.Government.Modules.Documents.Core.Mongo.Repositories;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Documents.Core.Mongo;

public static class Extensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services)
    {
        services.AddMongoRepository<Documents.DocumentDocument, Guid>("documents.documents");
        services.AddScoped<DocumentMongoRepository>();
        
        return services;
    }
}
