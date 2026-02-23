using Mamey.FWID.Identities.Api;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Mamey.FWID.Identities.Tests.EndToEnd.ApiEndpoints;

/// <summary>
/// Base class for API endpoint end-to-end tests with shared setup.
/// </summary>
[Collection("EndToEnd")]
public abstract class BaseApiEndpointTests : IClassFixture<PostgreSQLFixture>, IClassFixture<RedisFixture>, IClassFixture<MongoDBFixture>, IClassFixture<MinIOFixture>, IAsyncLifetime
{
    protected readonly PostgreSQLFixture PostgresFixture;
    protected readonly RedisFixture RedisFixture;
    protected readonly MongoDBFixture MongoFixture;
    protected readonly MinIOFixture MinIOFixture;
    protected WebApplicationFactory<Program> Factory { get; private set; } = null!;
    protected HttpClient Client { get; private set; } = null!;

    protected BaseApiEndpointTests(
        PostgreSQLFixture postgresFixture,
        RedisFixture redisFixture,
        MongoDBFixture mongoFixture,
        MinIOFixture minioFixture)
    {
        PostgresFixture = postgresFixture;
        RedisFixture = redisFixture;
        MongoFixture = mongoFixture;
        MinIOFixture = minioFixture;
    }

    public async Task InitializeAsync()
    {
        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    // Override configuration with test container connection strings
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        { "postgres:connectionString", PostgresFixture.ConnectionString },
                        { "redis:connectionString", RedisFixture.ConnectionString },
                        { "mongo:connectionString", MongoFixture.ConnectionString },
                        { "minio:endpoint", MinIOFixture.Endpoint },
                        { "minio:accessKey", MinIOFixture.AccessKey },
                        { "minio:secretKey", MinIOFixture.SecretKey }
                    });
                });
            });

        Client = Factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();
        Factory?.Dispose();
    }
}

