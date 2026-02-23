using Microsoft.Extensions.DependencyInjection;

namespace Mamey.FWID.Identities.Infrastructure.Ledger;

/// <summary>
/// Extension methods for registering ledger audit services.
/// </summary>
internal static class Extensions
{
    /// <summary>
    /// Adds ledger audit services for immutable audit trail logging.
    /// </summary>
    public static IServiceCollection AddLedgerAuditServices(this IServiceCollection services)
    {
        services.AddScoped<ILedgerAuditService, LedgerAuditService>();
        
        // Event handlers are auto-registered via AddMicroserviceSharedInfrastructure
        // which scans for IDomainEventHandler implementations
        
        return services;
    }
}
