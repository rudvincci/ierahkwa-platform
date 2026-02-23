using Mamey.FWID.Identities.Application.Clients;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Infrastructure.BackgroundServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.FWID.Identities.Infrastructure.Blockchain;

/// <summary>
/// Extension methods for registering resilient blockchain services.
/// </summary>
internal static class Extensions
{
    /// <summary>
    /// Configuration section name for batch ledger options.
    /// </summary>
    public const string BatchLedgerSectionName = "batchLedger";

    /// <summary>
    /// Adds resilient blockchain services to the service collection.
    /// </summary>
    public static IServiceCollection AddResilientBlockchainServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure resilient blockchain options from appsettings.json
        services.Configure<ResilientBlockchainOptions>(
            configuration.GetSection(ResilientBlockchainOptions.SectionName));

        // Register the resilient blockchain client
        services.AddScoped<IResilientBlockchainClient, ResilientBlockchainClient>();

        // Register the background sync service
        services.AddHostedService<BlockchainAccountSyncService>();

        // Add Government Identity blockchain integration
        services.AddGovernmentIdentityServices(configuration);

        // Add Batch Ledger processing for high-throughput event logging
        services.AddBatchLedgerServices(configuration);

        return services;
    }

    /// <summary>
    /// Adds resilient blockchain services with custom configuration action.
    /// </summary>
    public static IServiceCollection AddResilientBlockchainServices(
        this IServiceCollection services,
        Action<ResilientBlockchainOptions> configureOptions)
    {
        // Configure resilient blockchain options with custom action
        services.Configure(configureOptions);

        // Register the resilient blockchain client
        services.AddScoped<IResilientBlockchainClient, ResilientBlockchainClient>();

        // Register the background sync service
        services.AddHostedService<BlockchainAccountSyncService>();

        return services;
    }

    /// <summary>
    /// Adds Government Identity blockchain integration services.
    /// </summary>
    public static IServiceCollection AddGovernmentIdentityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure government identity options from appsettings.json
        services.Configure<GovernmentIdentityOptions>(
            configuration.GetSection(GovernmentIdentityOptions.SectionName));

        // Register the gRPC client for MameyNode GovernmentService
        services.AddSingleton<IGovernmentIdentityClient, GovernmentIdentityClient>();

        // Register the government identity service
        services.AddScoped<IGovernmentIdentityService, GovernmentIdentityService>();

        return services;
    }

    /// <summary>
    /// Adds batch ledger services for high-throughput event logging.
    /// </summary>
    public static IServiceCollection AddBatchLedgerServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure batch ledger options from appsettings.json
        services.Configure<BatchLedgerOptions>(
            configuration.GetSection(BatchLedgerSectionName));

        // Register the batch ledger client
        services.AddSingleton<IBatchLedgerClient, BatchLedgerClient>();

        // Register the background batch processor
        services.AddHostedService<BlockchainBatchProcessor>();

        return services;
    }
}
