using System.Reflection;
using Mamey.Modules;
using Mamey.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mamey.Persistence.SQL;

public class DbContextAppInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DbContextAppInitializer> _logger;
    private readonly DatabaseInitializationCompletionService _completionService;

    public DbContextAppInitializer(
        IServiceProvider serviceProvider, 
        ILogger<DbContextAppInitializer> logger,
        DatabaseInitializationCompletionService completionService)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _completionService = completionService;
    }
        
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // 1) Get all loaded assemblies
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // 2) Safely extract all loadable types
        var allTypes = assemblies.SelectMany(asm => {
            try
            {
                return asm.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types
                    .Where(t => t is not null)!
                    .Cast<Type>();
            }
        });

        // 3) Find all your DbContext types
        var dbContextTypes = allTypes
            .Where(t =>
                typeof(DbContext).IsAssignableFrom(t)
                && !t.IsInterface
                && !t.IsAbstract);
       
        using var scope = _serviceProvider.CreateScope();
        foreach (var dbContextType in dbContextTypes)
        {
            var serviceProvider = scope.ServiceProvider;
            var dbContext = serviceProvider.GetService(dbContextType) as DbContext;
       
            if (dbContext is null)
            {
                continue;
            }

            _logger.LogInformation(
                "Ensuring database for context {Context} exists...",
                dbContextType.Name);
#if DEBUG
            // In dev, you might drop & recreate to keep schema in sync with your model:
            // await dbContext.Database.EnsureDeletedAsync(cancellationToken);
            
            // EnsureCreatedAsync checks if ANY tables exist in the database, not schema-specific
            // Since multiple contexts share the same database with different schemas, we need to
            // check if tables exist in this context's schema specifically and force creation if needed
            var database = dbContext.Database;
            var model = dbContext.Model;
            var defaultSchema = model.GetDefaultSchema();
            
            bool schemaHasTables = false;
            if (!string.IsNullOrEmpty(defaultSchema) && await database.CanConnectAsync(cancellationToken))
            {
                try
                {
                    var connection = database.GetDbConnection();
                    var providerName = database.ProviderName ?? "";
                    
                    string schemaCheckSql = null;
                    if (providerName.Contains("Postgre") || providerName.Contains("Npgsql"))
                    {
                        // PostgreSQL-specific check: count tables in this specific schema
                        schemaCheckSql = $@"
                            SELECT COUNT(*)
                            FROM pg_class AS cls
                            JOIN pg_namespace AS ns ON ns.oid = cls.relnamespace
                            WHERE ns.nspname = '{defaultSchema}'
                            AND cls.relkind IN ('r', 'v', 'm', 'f', 'p')
                            AND NOT EXISTS (
                                SELECT 1 FROM pg_depend WHERE
                                    classid = (
                                        SELECT cls.oid
                                        FROM pg_class AS cls
                                        JOIN pg_namespace AS ns ON ns.oid = cls.relnamespace
                                        WHERE relname = 'pg_class' AND ns.nspname = 'pg_catalog'
                                    ) AND
                                    objid = cls.oid AND
                                    deptype IN ('e', 'x')
                            )";
                    }
                    
                    if (!string.IsNullOrEmpty(schemaCheckSql))
                    {
                        await connection.OpenAsync(cancellationToken);
                        using var command = connection.CreateCommand();
                        command.CommandText = schemaCheckSql;
                        var result = await command.ExecuteScalarAsync(cancellationToken);
                        var tableCount = result != null ? Convert.ToInt32(result) : 0;
                        schemaHasTables = tableCount > 0;
                        await connection.CloseAsync();
                    }
                }
                catch (Exception ex)
                {
                    // If check fails, assume schema doesn't have tables and proceed with creation
                    _logger.LogWarning(ex, "Failed to check if schema {Schema} has tables. Proceeding with creation.", defaultSchema);
                    schemaHasTables = false;
                }
            }
            
            // If schema doesn't have tables, try EnsureCreatedAsync first
            // If that doesn't work (because other schemas have tables), fall back to MigrateAsync
            if (!schemaHasTables)
            {
                try
                {
                    // Try EnsureCreatedAsync first - this works if no tables exist in the database
                    await dbContext.Database.EnsureCreatedAsync(cancellationToken);
                    
                    // Verify tables were actually created by checking again
                    if (!string.IsNullOrEmpty(defaultSchema) && await database.CanConnectAsync(cancellationToken))
                    {
                        var connection = database.GetDbConnection();
                        var providerName = database.ProviderName ?? "";
                        
                        if (providerName.Contains("Postgre") || providerName.Contains("Npgsql"))
                        {
                            await connection.OpenAsync(cancellationToken);
                            using var verifyCommand = connection.CreateCommand();
                            verifyCommand.CommandText = $@"
                                SELECT COUNT(*)
                                FROM pg_class AS cls
                                JOIN pg_namespace AS ns ON ns.oid = cls.relnamespace
                                WHERE ns.nspname = '{defaultSchema}'
                                AND cls.relkind IN ('r', 'v', 'm', 'f', 'p')
                                AND NOT EXISTS (
                                    SELECT 1 FROM pg_depend WHERE
                                        classid = (
                                            SELECT cls.oid
                                            FROM pg_class AS cls
                                            JOIN pg_namespace AS ns ON ns.oid = cls.relnamespace
                                            WHERE relname = 'pg_class' AND ns.nspname = 'pg_catalog'
                                        ) AND
                                        objid = cls.oid AND
                                        deptype IN ('e', 'x')
                                )";
                            var verifyResult = await verifyCommand.ExecuteScalarAsync(cancellationToken);
                            var verifyCount = verifyResult != null ? Convert.ToInt32(verifyResult) : 0;
                            await connection.CloseAsync();
                            
                            // If tables still don't exist, EnsureCreatedAsync skipped due to other tables
                            // Create tables directly using the model differ and SQL generator
                            if (verifyCount == 0)
                            {
                                _logger.LogInformation("EnsureCreatedAsync skipped table creation (other schemas have tables). Creating tables directly for {Context}.", dbContextType.Name);
                                
                                // Ensure the schema exists first
                                if (!string.IsNullOrEmpty(defaultSchema) && providerName.Contains("Postgre"))
                                {
                                    await connection.OpenAsync(cancellationToken);
                                    try
                                    {
                                        using var schemaCommand = connection.CreateCommand();
                                        schemaCommand.CommandText = $@"
                                            DO $EF$
                                            BEGIN
                                                IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = '{defaultSchema}') THEN
                                                    CREATE SCHEMA {defaultSchema};
                                                END IF;
                                            END $EF$;";
                                        await schemaCommand.ExecuteNonQueryAsync(cancellationToken);
                                    }
                                    finally
                                    {
                                        await connection.CloseAsync();
                                    }
                                }
                                
                                // Try to use IRelationalDatabaseCreator to create tables directly
                                // This avoids the read-optimized model issue with IMigrationsModelDiffer
                                var databaseCreator = database.GetService<Microsoft.EntityFrameworkCore.Storage.IRelationalDatabaseCreator>();
                                
                                if (databaseCreator != null)
                                {
                                    try
                                    {
                                        // IRelationalDatabaseCreator.CreateTables() creates tables from the model
                                        // This should work even with read-optimized models
                                        await databaseCreator.CreateTablesAsync(cancellationToken);
                                        _logger.LogInformation("Successfully created tables for {Context} schema {Schema} using IRelationalDatabaseCreator.", dbContextType.Name, defaultSchema);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogWarning(ex, "Failed to create tables for {Context} using IRelationalDatabaseCreator. Trying MigrateAsync fallback.", dbContextType.Name);
                                        
                                        // Fallback: Use MigrateAsync even in DEBUG mode
                                        try
                                        {
                                            await dbContext.Database.MigrateAsync(cancellationToken);
                                            _logger.LogInformation("Successfully created tables for {Context} using MigrateAsync fallback.", dbContextType.Name);
                                        }
                                        catch (Exception migrateEx)
                                        {
                                            // MigrateAsync might fail if no migrations exist
                                            if (migrateEx.Message.Contains("migration") || migrateEx.Message.Contains("Migration"))
                                            {
                                                _logger.LogWarning("MigrateAsync fallback failed for {Context} (likely no migrations exist). Tables were not created. Consider creating an initial migration.", dbContextType.Name);
                                            }
                                            else
                                            {
                                                _logger.LogError(migrateEx, "Failed to create tables for {Context} using MigrateAsync fallback.", dbContextType.Name);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // If IRelationalDatabaseCreator is not available, try MigrateAsync
                                    _logger.LogWarning("IRelationalDatabaseCreator not available for {Context}. Trying MigrateAsync.", dbContextType.Name);
                                    try
                                    {
                                        await dbContext.Database.MigrateAsync(cancellationToken);
                                        _logger.LogInformation("Successfully created tables for {Context} using MigrateAsync.", dbContextType.Name);
                                    }
                                    catch (Exception migrateEx)
                                    {
                                        if (migrateEx.Message.Contains("migration") || migrateEx.Message.Contains("Migration"))
                                        {
                                            _logger.LogWarning("MigrateAsync failed for {Context} (likely no migrations exist). Tables were not created. Consider creating an initial migration.", dbContextType.Name);
                                        }
                                        else
                                        {
                                            _logger.LogError(migrateEx, "Failed to create tables for {Context} using MigrateAsync.", dbContextType.Name);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // If EnsureCreatedAsync fails, log and continue
                    // The schema check will catch this on next run
                    _logger.LogWarning(ex, "EnsureCreatedAsync failed for {Context}. Tables may not have been created.", dbContextType.Name);
                }
            }
            else
            {
                _logger.LogInformation("Schema {Schema} already has tables. Skipping table creation for {Context}.", defaultSchema ?? "default", dbContextType.Name);
            }
#else
            // In production, apply any pending migrations:
            await dbContext.Database.MigrateAsync(cancellationToken);
#endif
            _logger.LogInformation("Running DB context for module {Module}...", dbContextType.GetModuleName(splitIndex:3));
        }
        
        // Signal completion after all databases are initialized
        _completionService.SignalCompletion();
        _logger.LogInformation("Database initialization completed for all contexts.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
