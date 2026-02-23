namespace Mamey.Government.Shared.Abstractions.Seeding;

/// <summary>
/// Interface for module data seeders.
/// Allows the bootstrapper to seed data for individual modules.
/// </summary>
public interface IModuleSeeder
{
    /// <summary>
    /// Order in which the seeder should run.
    /// Lower values run first. Default is 100.
    /// </summary>
    int Order => 100;
    
    /// <summary>
    /// Name of the module being seeded.
    /// </summary>
    string ModuleName { get; }
    
    /// <summary>
    /// Seeds the database with initial data.
    /// </summary>
    Task SeedAsync(CancellationToken cancellationToken = default);
}
