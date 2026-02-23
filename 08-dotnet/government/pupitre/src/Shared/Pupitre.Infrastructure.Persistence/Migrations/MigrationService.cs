using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pupitre.Infrastructure.Persistence.Migrations;

/// <summary>
/// Generic migration service for EF Core DbContexts.
/// </summary>
public class MigrationService<TContext> : IMigrationService where TContext : DbContext
{
    private readonly TContext _context;
    private readonly ILogger<MigrationService<TContext>> _logger;

    public MigrationService(TContext context, ILogger<MigrationService<TContext>> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        var contextName = typeof(TContext).Name;
        _logger.LogInformation("Starting database migration for {Context}", contextName);

        try
        {
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync(cancellationToken);
            var pendingCount = pendingMigrations.Count();

            if (pendingCount == 0)
            {
                _logger.LogInformation("No pending migrations for {Context}", contextName);
                return;
            }

            _logger.LogInformation("Applying {Count} pending migration(s) for {Context}: {Migrations}",
                pendingCount, contextName, string.Join(", ", pendingMigrations));

            await _context.Database.MigrateAsync(cancellationToken);

            _logger.LogInformation("Successfully applied {Count} migration(s) for {Context}",
                pendingCount, contextName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply migrations for {Context}", contextName);
            throw;
        }
    }

    public async Task<bool> HasPendingMigrationsAsync(CancellationToken cancellationToken = default)
    {
        var pending = await _context.Database.GetPendingMigrationsAsync(cancellationToken);
        return pending.Any();
    }

    public async Task<IEnumerable<string>> GetPendingMigrationsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Database.GetPendingMigrationsAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> GetAppliedMigrationsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Database.GetAppliedMigrationsAsync(cancellationToken);
    }
}
