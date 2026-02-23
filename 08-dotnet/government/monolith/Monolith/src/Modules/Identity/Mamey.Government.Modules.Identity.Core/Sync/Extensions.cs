using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Identity.Core.Sync;

/// <summary>
/// Extension methods for registering read model sync services.
/// </summary>
internal static class Extensions
{
    /// <summary>
    /// Adds read model sync services for event-driven replication from PostgreSQL to MongoDB.
    /// </summary>
    public static IServiceCollection AddReadModelSyncServices(this IServiceCollection services)
    {
        services.AddScoped<IReadModelSyncService, ReadModelSyncService>();
        
        // Event handlers are auto-registered via AddMicroserviceSharedInfrastructure
        // which scans for IDomainEventHandler implementations
        
        return services;
    }
}
