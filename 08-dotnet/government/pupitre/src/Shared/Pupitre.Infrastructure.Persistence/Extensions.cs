using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Infrastructure.Persistence.Migrations;

namespace Pupitre.Infrastructure.Persistence;

public static class Extensions
{
    /// <summary>
    /// Adds EF Core DbContext with PostgreSQL provider.
    /// </summary>
    public static IServiceCollection AddPupitreDbContext<TContext>(
        this IServiceCollection services,
        string connectionString,
        Action<DbContextOptionsBuilder>? optionsAction = null)
        where TContext : DbContext
    {
        services.AddDbContext<TContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
                npgsql.CommandTimeout(30);
            });

            optionsAction?.Invoke(options);
        });

        return services;
    }

    /// <summary>
    /// Adds migration service for the specified DbContext.
    /// </summary>
    public static IServiceCollection AddMigrationService<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddScoped<IMigrationService, MigrationService<TContext>>();
        return services;
    }

    /// <summary>
    /// Adds hosted service to run migrations on startup.
    /// </summary>
    public static IServiceCollection AddMigrationHostedService(this IServiceCollection services)
    {
        services.AddHostedService<MigrationHostedService>();
        return services;
    }

    /// <summary>
    /// Configures automatic migrations for the specified DbContext.
    /// </summary>
    public static IServiceCollection AddPupitreDbContextWithMigrations<TContext>(
        this IServiceCollection services,
        string connectionString,
        Action<DbContextOptionsBuilder>? optionsAction = null)
        where TContext : DbContext
    {
        services.AddPupitreDbContext<TContext>(connectionString, optionsAction);
        services.AddMigrationService<TContext>();
        return services;
    }
}
