using Mamey.Government.Modules.Passports.Core.Domain.Repositories;
using Mamey.Government.Modules.Passports.Core.EF.Repositories;
using Mamey.Government.Modules.Passports.Core.Mongo.Repositories;
using Mamey.Government.Modules.Passports.Core.Redis.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Passports.Core.Composite;

internal static class Extensions
{
    public static IServiceCollection AddCompositeRepositories(this IServiceCollection services)
    {
        services.AddScoped<PassportPostgresRepository>();
        services.AddScoped<PassportMongoRepository>();
        services.AddScoped<PassportRedisRepository>();
        services.AddScoped<IPassportRepository, CompositePassportRepository>();
        
        return services;
    }
}
