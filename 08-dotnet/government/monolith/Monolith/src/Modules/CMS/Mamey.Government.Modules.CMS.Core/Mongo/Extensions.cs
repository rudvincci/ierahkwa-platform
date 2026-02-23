using Mamey.Government.Modules.CMS.Core.Mongo.Repositories;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.CMS.Core.Mongo;

public static class Extensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services)
    {
        services.AddMongoRepository<Documents.ContentDocument, Guid>("cms.contents");
        services.AddScoped<ContentMongoRepository>();
        
        return services;
    }
}
