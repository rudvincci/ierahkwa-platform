using Mamey.Persistence.SQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Mamey.Postgres;

public static class Extensions
{
    public static IServiceCollection AddPostgres(this IServiceCollection services)
    {

        services.ConfigurePostgres();

        return services;
    }

    public static IServiceCollection AddPostgres<T>(this IServiceCollection services, Action<NpgsqlDbContextOptionsBuilder>? postgresSqlOptions = null) where T : DbContext
    {
        var options = services.GetOptions<PostgresOptions>("postgres");
        services.ConfigurePostgres();
        services.AddDbContext<T>(x => x.UseNpgsql(options.ConnectionString, postgresSqlOptions));

        return services;
    }

    private static IServiceCollection ConfigurePostgres(this IServiceCollection services)
    {
        var options = services.GetOptions<PostgresOptions>("postgres");
        services.AddSingleton(options);
        services.AddSingleton(new UnitOfWorkTypeRegistry());
        // Temporary fix for EF Core issue related to https://github.com/npgsql/efcore.pg/issues/2000
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        return services;
    }

    public static IServiceCollection AddUnitOfWork<T>(this IServiceCollection services) where T : class, IUnitOfWork
    {
        services.AddScoped<IUnitOfWork, T>();
        services.AddScoped<T>();
        using var serviceProvider = services.BuildServiceProvider();
        serviceProvider.GetRequiredService<UnitOfWorkTypeRegistry>().Register<T>();

        return services;
    }
}
