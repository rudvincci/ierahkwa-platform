using Mamey.Government.Modules.CMS.Core.Domain.Repositories;
using Mamey.Government.Modules.CMS.Core.EF.Repositories;
using Mamey.Government.Modules.CMS.Core.Mongo.Repositories;
using Mamey.Government.Modules.CMS.Core.Redis.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.CMS.Core.Composite;

internal static class Extensions
{
    public static IServiceCollection AddCompositeRepositories(this IServiceCollection services)
    {
        services.AddScoped<ContentPostgresRepository>();
        services.AddScoped<ContentMongoRepository>();
        services.AddScoped<ContentRedisRepository>();
        services.AddScoped<IContentRepository, CompositeContentRepository>();
        
        return services;
    }
}
