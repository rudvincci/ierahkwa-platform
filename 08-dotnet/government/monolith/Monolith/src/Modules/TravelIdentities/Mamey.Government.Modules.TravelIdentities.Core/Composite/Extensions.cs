using Mamey.Government.Modules.TravelIdentities.Core.Domain.Repositories;
using Mamey.Government.Modules.TravelIdentities.Core.EF.Repositories;
using Mamey.Government.Modules.TravelIdentities.Core.Mongo.Repositories;
using Mamey.Government.Modules.TravelIdentities.Core.Redis.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.TravelIdentities.Core.Composite;

internal static class Extensions
{
    public static IServiceCollection AddCompositeRepositories(this IServiceCollection services)
    {
        services.AddScoped<TravelIdentityPostgresRepository>();
        services.AddScoped<TravelIdentityMongoRepository>();
        services.AddScoped<TravelIdentityRedisRepository>();
        services.AddScoped<ITravelIdentityRepository, CompositeTravelIdentityRepository>();
        
        return services;
    }
}
