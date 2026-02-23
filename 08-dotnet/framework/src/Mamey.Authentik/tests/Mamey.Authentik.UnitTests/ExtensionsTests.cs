using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mamey.Authentik;
using Mamey.Authentik.Caching;
using Mamey.Authentik.Services;
using Xunit;
using Microsoft.Extensions.Caching.Distributed;

namespace Mamey.Authentik.UnitTests;

public class ExtensionsTests
{
    [Fact]
    public void AddAuthentik_WithAction_RegistersAllServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddAuthentik(options =>
        {
            options.BaseUrl = "https://test.authentik.local";
            options.ApiToken = "test-token";
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<IAuthentikClient>().Should().NotBeNull();
        serviceProvider.GetService<IAuthentikAdminService>().Should().NotBeNull();
        serviceProvider.GetService<IAuthentikCoreService>().Should().NotBeNull();
        serviceProvider.GetService<IAuthentikOAuth2Service>().Should().NotBeNull();
        serviceProvider.GetService<IAuthentikFlowsService>().Should().NotBeNull();
        serviceProvider.GetService<IAuthentikPoliciesService>().Should().NotBeNull();
        serviceProvider.GetService<IAuthentikProvidersService>().Should().NotBeNull();
        serviceProvider.GetService<IAuthentikStagesService>().Should().NotBeNull();
        serviceProvider.GetService<IAuthentikSourcesService>().Should().NotBeNull();
        serviceProvider.GetService<IAuthentikEventsService>().Should().NotBeNull();
        serviceProvider.GetService<IOptions<AuthentikOptions>>().Should().NotBeNull();
        serviceProvider.GetService<IAuthentikCache>().Should().NotBeNull();
    }

    [Fact]
    public void AddAuthentik_WithConfiguration_RegistersAllServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Authentik:BaseUrl", "https://test.authentik.local" },
                { "Authentik:ApiToken", "test-token" }
            })
            .Build();

        // Act
        services.AddAuthentik(configuration.GetSection("Authentik"));

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<IAuthentikClient>().Should().NotBeNull();
        var options = serviceProvider.GetRequiredService<IOptions<AuthentikOptions>>().Value;
        options.BaseUrl.Should().Be("https://test.authentik.local");
        options.ApiToken.Should().Be("test-token");
    }

    [Fact]
    public void AddAuthentikDistributedCache_ReplacesInMemoryCache()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDistributedMemoryCache();
        
        services.AddAuthentik(options =>
        {
            options.BaseUrl = "https://test.authentik.local";
            options.ApiToken = "test-token";
        });

        // Act
        services.AddAuthentikDistributedCache();

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var cache = serviceProvider.GetService<IAuthentikCache>();
        cache.Should().NotBeNull();
        cache.Should().BeOfType<DistributedAuthentikCache>();
    }
}
