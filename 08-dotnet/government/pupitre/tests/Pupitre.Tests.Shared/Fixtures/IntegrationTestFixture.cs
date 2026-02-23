using Testcontainers.PostgreSql;
using Testcontainers.MongoDb;
using Testcontainers.Redis;
using Testcontainers.RabbitMq;
using Xunit;

namespace Pupitre.Tests.Shared.Fixtures;

/// <summary>
/// Full integration test fixture with all dependencies.
/// </summary>
public sealed class IntegrationTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres;
    private readonly MongoDbContainer _mongo;
    private readonly RedisContainer _redis;
    private readonly RabbitMqContainer _rabbitmq;

    public string PostgresConnectionString => _postgres.GetConnectionString();
    public string MongoConnectionString => _mongo.GetConnectionString();
    public string RedisConnectionString => _redis.GetConnectionString();
    public string RabbitMqConnectionString => _rabbitmq.GetConnectionString();

    public IntegrationTestFixture()
    {
        _postgres = new PostgreSqlBuilder()
            .WithImage("timescale/timescaledb:latest-pg16")
            .WithDatabase("pupitre_test")
            .WithUsername("test")
            .WithPassword("test")
            .Build();

        _mongo = new MongoDbBuilder()
            .WithImage("mongo:7")
            .Build();

        _redis = new RedisBuilder()
            .WithImage("redis:7-alpine")
            .Build();

        _rabbitmq = new RabbitMqBuilder()
            .WithImage("rabbitmq:3.12-management-alpine")
            .Build();
    }

    public async Task InitializeAsync()
    {
        // Start all containers in parallel
        await Task.WhenAll(
            _postgres.StartAsync(),
            _mongo.StartAsync(),
            _redis.StartAsync(),
            _rabbitmq.StartAsync()
        );
    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(
            _postgres.DisposeAsync().AsTask(),
            _mongo.DisposeAsync().AsTask(),
            _redis.DisposeAsync().AsTask(),
            _rabbitmq.DisposeAsync().AsTask()
        );
    }
}

/// <summary>
/// Collection fixture for sharing all containers across integration tests.
/// </summary>
[CollectionDefinition("Integration")]
public class IntegrationCollection : ICollectionFixture<IntegrationTestFixture>
{
}
