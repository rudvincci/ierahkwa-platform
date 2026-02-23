using Testcontainers.Redis;
using Xunit;

namespace Pupitre.Tests.Shared.Fixtures;

/// <summary>
/// Redis test fixture using Testcontainers.
/// </summary>
public sealed class RedisFixture : IAsyncLifetime
{
    private readonly RedisContainer _container;

    public string ConnectionString => _container.GetConnectionString();

    public RedisFixture()
    {
        _container = new RedisBuilder()
            .WithImage("redis:7-alpine")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}

/// <summary>
/// Collection fixture for sharing Redis container across tests.
/// </summary>
[CollectionDefinition("Redis")]
public class RedisCollection : ICollectionFixture<RedisFixture>
{
}
