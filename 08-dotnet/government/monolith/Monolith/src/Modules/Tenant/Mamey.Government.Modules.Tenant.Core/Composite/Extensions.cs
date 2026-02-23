using Mamey.Government.Modules.Tenant.Core.Domain.Repositories;
using Mamey.Government.Modules.Tenant.Core.EF.Repositories;
using Mamey.Government.Modules.Tenant.Core.Mongo.Repositories;
using Mamey.Government.Modules.Tenant.Core.Redis.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Tenant.Core.Composite;

internal static class Extensions
{
    public static IServiceCollection AddCompositeRepositories(this IServiceCollection services)
    {
        services.AddScoped<TenantPostgresRepository>();
        services.AddScoped<TenantMongoRepository>();
        services.AddScoped<TenantRedisRepository>();
        services.AddScoped<ITenantRepository, CompositeTenantRepository>();
        
        return services;
    }
}
