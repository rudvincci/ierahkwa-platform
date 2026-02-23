using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Testcontainers.Redis;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Shared.Fixtures;

/// <summary>
/// Test fixture for Redis integration tests using Testcontainers.NET.
/// </summary>
public class RedisFixture : IAsyncLifetime
{
    private readonly RedisContainer _redisContainer;
    public IServiceProvider ServiceProvider { get; private set; } = null!;
    public string ConnectionString { get; private set; } = string.Empty;
    public IConnectionMultiplexer ConnectionMultiplexer { get; private set; } = null!;
    public IDatabase Database { get; private set; } = null!;

    public RedisFixture()
    {
        _redisContainer = new RedisBuilder()
            .WithImage("redis:7-alpine")
            .WithPortBinding(6379, true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        // Start the Redis container
        await _redisContainer.StartAsync();
        ConnectionString = _redisContainer.GetConnectionString();

        // Configure services
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));

        // Add Redis connection
        ConnectionMultiplexer = await StackExchange.Redis.ConnectionMultiplexer.ConnectAsync(ConnectionString);
        Database = ConnectionMultiplexer.GetDatabase();

        services.AddSingleton(ConnectionMultiplexer);
        services.AddSingleton(Database);

        ServiceProvider = services.BuildServiceProvider();

        // Flush database to ensure clean state
        await Database.ExecuteAsync("FLUSHDB");
    }

    public async Task DisposeAsync()
    {
        // Flush database before disposal
        if (Database != null)
        {
            await Database.ExecuteAsync("FLUSHDB");
        }

        ConnectionMultiplexer?.Dispose();
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
        await _redisContainer.DisposeAsync();
    }
}

