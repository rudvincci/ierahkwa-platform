using Mamey.Government.Shared.Abstractions.Seeding;

namespace Mamey.Government.BlazorServer.Seeding;

/// <summary>
/// Orchestrates data seeding across all modules in the correct order.
/// </summary>
public class DataSeederOrchestrator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataSeederOrchestrator> _logger;

    public DataSeederOrchestrator(IServiceProvider serviceProvider, ILogger<DataSeederOrchestrator> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Seeds all module data in the correct dependency order.
    /// </summary>
    public async Task SeedAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting data seeding...");
        
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var scopedProvider = scope.ServiceProvider;

            // Get all registered module seeders
            var seeders = scopedProvider.GetServices<IModuleSeeder>()
                .OrderBy(s => s.Order)
                .ToList();

            if (seeders.Count == 0)
            {
                _logger.LogWarning("No module seeders registered");
                return;
            }

            _logger.LogInformation("Found {Count} module seeders", seeders.Count);

            foreach (var seeder in seeders)
            {
                _logger.LogInformation("Seeding {Module} (order: {Order})...", seeder.ModuleName, seeder.Order);
                
                try
                {
                    await seeder.SeedAsync(cancellationToken);
                    _logger.LogInformation("Completed seeding {Module}", seeder.ModuleName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to seed {Module}", seeder.ModuleName);
                    throw;
                }
            }

            _logger.LogInformation("Data seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Data seeding failed");
            throw;
        }
    }
}
