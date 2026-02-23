using Mamey.Government.Modules.TravelIdentities.Core.Mongo.Repositories;
using Mamey.Government.Modules.TravelIdentities.Core.Mongo.Documents;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.TravelIdentities.Core.Mongo;

public static class Extensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services)
    {
        services.AddMongoRepository<TravelIdentityDocument, Guid>("travel_identities.travel_identities");
        services.AddScoped<TravelIdentityMongoRepository>();
        
        return services;
    }
}
