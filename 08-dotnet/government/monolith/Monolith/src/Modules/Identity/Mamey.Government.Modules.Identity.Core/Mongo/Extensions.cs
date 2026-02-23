using Mamey.Government.Modules.Identity.Core.Mongo.Repositories;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Identity.Core.Mongo;

public static class Extensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services)
    {
        services.AddMongoRepository<Documents.UserProfileDocument, Guid>("identity.userprofiles");
        services.AddScoped<UserProfileMongoRepository>();
        
        return services;
    }
}
