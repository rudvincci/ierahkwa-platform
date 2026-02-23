using Mamey.Government.Modules.Tenant.Core.Mongo.Repositories;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Tenant.Core.Mongo;

public static class Extensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services)
    {
        services.AddMongoRepository<Documents.TenantDocument, Guid>("tenant.tenants");
        services.AddScoped<TenantMongoRepository>();
        
        return services;
    }
}
