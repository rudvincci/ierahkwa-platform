using Testcontainers.MongoDb;
using Xunit;

namespace Pupitre.Tests.Shared.Fixtures;

/// <summary>
/// MongoDB test fixture using Testcontainers.
/// </summary>
public sealed class MongoFixture : IAsyncLifetime
{
    private readonly MongoDbContainer _container;

    public string ConnectionString => _container.GetConnectionString();

    public MongoFixture()
    {
        _container = new MongoDbBuilder()
            .WithImage("mongo:7")
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
/// Collection fixture for sharing MongoDB container across tests.
/// </summary>
[CollectionDefinition("Mongo")]
public class MongoCollection : ICollectionFixture<MongoFixture>
{
}
