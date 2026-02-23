using Mamey.Government.Modules.Citizens.Core.Mongo.Repositories;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Citizens.Core.Mongo;

public static class Extensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services)
    {
        services.AddMongoRepository<Documents.CitizenDocument, Guid>("citizens.citizens");
        services.AddScoped<CitizenMongoRepository>();
        
        return services;
    }
}
