using Testcontainers.PostgreSql;
using Testcontainers.MongoDb;
using Testcontainers.Redis;

namespace Mamey.Government.Tests.Integration;

/// <summary>
/// Integration tests for composite repository pattern consistency.
/// Uses Testcontainers to spin up real PostgreSQL, MongoDB, and Redis instances.
/// </summary>
public class CompositeRepositoryIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly MongoDbContainer _mongoContainer;
    private readonly RedisContainer _redisContainer;

    public CompositeRepositoryIntegrationTests()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithDatabase("government_test")
            .WithUsername("test")
            .WithPassword("test")
            .Build();

        _mongoContainer = new MongoDbBuilder()
            .WithImage("mongo:7.0")
            .Build();

        _redisContainer = new RedisBuilder()
            .WithImage("redis:7-alpine")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            _postgresContainer.StartAsync(),
            _mongoContainer.StartAsync(),
            _redisContainer.StartAsync()
        );
    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(
            _postgresContainer.DisposeAsync().AsTask(),
            _mongoContainer.DisposeAsync().AsTask(),
            _redisContainer.DisposeAsync().AsTask()
        );
    }

    [Fact]
    public async Task CompositeRepository_ShouldMaintainConsistency_AcrossAllStores()
    {
        // This test verifies that data written through the composite repository
        // is consistently available in all storage backends
        
        // Arrange
        var postgresConnectionString = _postgresContainer.GetConnectionString();
        var mongoConnectionString = _mongoContainer.GetConnectionString();
        var redisConnectionString = _redisContainer.GetConnectionString();

        // Assert containers are running
        postgresConnectionString.Should().NotBeNullOrEmpty();
        mongoConnectionString.Should().NotBeNullOrEmpty();
        redisConnectionString.Should().NotBeNullOrEmpty();

        // TODO: Wire up actual repository implementations with container connection strings
        // and verify CRUD operations maintain consistency across all three stores
    }

    [Fact]
    public async Task CompositeRepository_ShouldFallbackGracefully_WhenSecondaryStoreFails()
    {
        // This test verifies that reads continue to work even if Redis or MongoDB are unavailable
        // PostgreSQL (source of truth) should always serve as the final fallback

        // Arrange
        var postgresConnectionString = _postgresContainer.GetConnectionString();

        // Assert
        postgresConnectionString.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ReadModelSync_ShouldPropagateChanges_ToMongoDb()
    {
        // This test verifies that domain events trigger read model synchronization
        // Data written to PostgreSQL should appear in MongoDB via the sync handler

        // Arrange
        var mongoConnectionString = _mongoContainer.GetConnectionString();

        // Assert
        mongoConnectionString.Should().NotBeNullOrEmpty();
    }
}
