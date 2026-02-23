using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mamey.FWID.Identities.Infrastructure.EF;
using Testcontainers.PostgreSql;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Shared.Fixtures;

/// <summary>
/// Test fixture for PostgreSQL integration tests using Testcontainers.NET.
/// </summary>
public class PostgreSQLFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    public IServiceProvider ServiceProvider { get; private set; } = null!;
    public string ConnectionString { get; private set; } = string.Empty;

    public PostgreSQLFixture()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15")
            .WithDatabase("fwid_identities_test")
            .WithUsername("test")
            .WithPassword("test")
            .WithPortBinding(5432, true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        // Start the PostgreSQL container
        await _postgresContainer.StartAsync();
        ConnectionString = _postgresContainer.GetConnectionString();

        // Configure services
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));

        // Add Entity Framework Core
        services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseNpgsql(ConnectionString);
            options.EnableSensitiveDataLogging();
        });

        ServiceProvider = services.BuildServiceProvider();

        // Ensure database is created
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        await context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        if (ServiceProvider != null)
        {
            using var scope = ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
            await context.Database.EnsureDeletedAsync();
        }

        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
        await _postgresContainer.DisposeAsync();
    }
}

