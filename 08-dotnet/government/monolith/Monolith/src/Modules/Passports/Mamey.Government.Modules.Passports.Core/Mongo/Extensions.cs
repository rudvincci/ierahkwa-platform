using Mamey.Government.Modules.Passports.Core.Mongo.Repositories;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Passports.Core.Mongo;

public static class Extensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services)
    {
        services.AddMongoRepository<Documents.PassportDocument, Guid>("passports.passports");
        services.AddScoped<PassportMongoRepository>();
        
        return services;
    }
}
