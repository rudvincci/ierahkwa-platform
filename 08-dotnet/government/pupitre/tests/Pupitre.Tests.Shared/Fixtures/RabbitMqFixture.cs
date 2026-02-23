using Testcontainers.RabbitMq;
using Xunit;

namespace Pupitre.Tests.Shared.Fixtures;

/// <summary>
/// RabbitMQ test fixture using Testcontainers.
/// </summary>
public sealed class RabbitMqFixture : IAsyncLifetime
{
    private readonly RabbitMqContainer _container;

    public string ConnectionString => _container.GetConnectionString();

    public RabbitMqFixture()
    {
        _container = new RabbitMqBuilder()
            .WithImage("rabbitmq:3.12-management-alpine")
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
/// Collection fixture for sharing RabbitMQ container across tests.
/// </summary>
[CollectionDefinition("RabbitMQ")]
public class RabbitMqCollection : ICollectionFixture<RabbitMqFixture>
{
}
