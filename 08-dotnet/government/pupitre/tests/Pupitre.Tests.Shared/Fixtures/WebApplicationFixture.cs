using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Pupitre.Tests.Shared.Fixtures;

public class WebApplicationFixture<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime
    where TProgram : class
{
    private readonly PostgresFixture _postgresFixture;
    private readonly MongoFixture _mongoFixture;
    private readonly RedisFixture _redisFixture;

    public WebApplicationFixture()
    {
        _postgresFixture = new PostgresFixture();
        _mongoFixture = new MongoFixture();
        _redisFixture = new RedisFixture();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Override connection strings with test containers
            // This would be configured per-service
        });

        builder.UseEnvironment("Testing");
    }

    public async Task InitializeAsync()
    {
        await _postgresFixture.InitializeAsync();
        await _mongoFixture.InitializeAsync();
        await _redisFixture.InitializeAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgresFixture.DisposeAsync();
        await _mongoFixture.DisposeAsync();
        await _redisFixture.DisposeAsync();
        await base.DisposeAsync();
    }
}
