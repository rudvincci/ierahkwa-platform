using Mamey.Government.Modules.Citizens.Core.Domain.Repositories;
using Mamey.Government.Modules.Citizens.Core.EF.Repositories;
using Mamey.Government.Modules.Citizens.Core.Mongo.Repositories;
using Mamey.Government.Modules.Citizens.Core.Redis.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Citizens.Core.Composite;

internal static class Extensions
{
    public static IServiceCollection AddCompositeRepositories(this IServiceCollection services)
    {
        services.AddScoped<CitizenPostgresRepository>();
        services.AddScoped<CitizenMongoRepository>();
        services.AddScoped<CitizenRedisRepository>();
        services.AddScoped<ICitizenRepository, CompositeCitizenRepository>();
        
        return services;
    }
}
