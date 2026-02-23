namespace Pupitre.Infrastructure.Persistence.Migrations;

/// <summary>
/// Service interface for database migrations.
/// </summary>
public interface IMigrationService
{
    /// <summary>
    /// Applies all pending migrations to the database.
    /// </summary>
    Task MigrateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if there are any pending migrations.
    /// </summary>
    Task<bool> HasPendingMigrationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of pending migration names.
    /// </summary>
    Task<IEnumerable<string>> GetPendingMigrationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of applied migration names.
    /// </summary>
    Task<IEnumerable<string>> GetAppliedMigrationsAsync(CancellationToken cancellationToken = default);
}
