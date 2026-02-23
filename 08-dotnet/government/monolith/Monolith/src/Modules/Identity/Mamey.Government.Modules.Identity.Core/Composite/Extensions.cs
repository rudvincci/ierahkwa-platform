using Mamey.Government.Modules.Identity.Core.Domain.Repositories;
using Mamey.Government.Modules.Identity.Core.EF.Repositories;
using Mamey.Government.Modules.Identity.Core.Mongo.Repositories;
using Mamey.Government.Modules.Identity.Core.Redis.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Identity.Core.Composite;

internal static class Extensions
{
    public static IServiceCollection AddCompositeRepositories(this IServiceCollection services)
    {
        // Register individual repositories (for composite to use)
        services.AddScoped<UserProfilePostgresRepository>();
        services.AddScoped<UserProfileMongoRepository>();
        services.AddScoped<UserProfileRedisRepository>();
        
        // Register composite repository as the interface
        services.AddScoped<IUserProfileRepository, CompositeUserProfileRepository>();
        
        return services;
    }
}
