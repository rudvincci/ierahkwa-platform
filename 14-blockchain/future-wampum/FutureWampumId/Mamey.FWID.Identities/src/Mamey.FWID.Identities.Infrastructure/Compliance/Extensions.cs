using Mamey.FWID.Identities.Application.Clients;
using Mamey.FWID.Identities.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.FWID.Identities.Infrastructure.Compliance;

/// <summary>
/// Extension methods for registering compliance services.
/// </summary>
internal static class Extensions
{
    /// <summary>
    /// Adds compliance services to the service collection.
    /// </summary>
    public static IServiceCollection AddComplianceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure compliance options from appsettings.json
        services.Configure<ComplianceOptions>(
            configuration.GetSection(ComplianceOptions.SectionName));

        // Register the gRPC client for MameyNode ComplianceService
        services.AddSingleton<IComplianceClient, ComplianceClient>();

        // Register the audit trail service
        services.AddScoped<IAuditTrailService, AuditTrailService>();

        return services;
    }
}
