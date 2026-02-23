using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;
using Mamey.Image;
using Mamey.Image.BackgroundRemoval;

namespace Mamey.Image.BackgroundRemoval.Tests;

public class ExtensionsTests
{
    [Fact]
    public void AddMameyImageBackgroundRemoval_WithConfiguration_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["BackgroundRemovalApi:BaseUrl"] = "http://localhost:5000",
                ["BackgroundRemovalApi:TimeoutSeconds"] = "30"
            })
            .Build();

        // Act
        services.AddMameyImageBackgroundRemoval(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        // Check that options are registered
        var options = serviceProvider.GetService<IOptions<BackgroundRemovalOptions>>();
        Assert.NotNull(options);
        Assert.Equal("http://localhost:5000", options.Value.BaseUrl);
        Assert.Equal(30, options.Value.TimeoutSeconds);

        // Check that HTTP client is registered
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        Assert.NotNull(httpClientFactory);
    }

    [Fact]
    public void AddMameyImageBackgroundRemoval_WithConfigureOptions_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var baseUrl = "http://test:8080";
        var timeoutSeconds = 60;

        // Act
        services.AddMameyImageBackgroundRemoval(options =>
        {
            options.BaseUrl = baseUrl;
            options.TimeoutSeconds = timeoutSeconds;
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        // Check that options are registered
        var options = serviceProvider.GetService<IOptions<BackgroundRemovalOptions>>();
        Assert.NotNull(options);
        Assert.Equal(baseUrl, options.Value.BaseUrl);
        Assert.Equal(timeoutSeconds, options.Value.TimeoutSeconds);

        // Check that HTTP client is registered
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        Assert.NotNull(httpClientFactory);
    }

    [Fact]
    public void AddMameyImageBackgroundRemoval_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            Extensions.AddMameyImageBackgroundRemoval(null!, new ConfigurationBuilder().Build()));
    }

    [Fact]
    public void AddMameyImageBackgroundRemoval_WithNullConfiguration_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            Extensions.AddMameyImageBackgroundRemoval(services, (IConfiguration)null!));
    }

    [Fact]
    public void AddMameyImageBackgroundRemoval_WithNullConfigureOptions_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            Extensions.AddMameyImageBackgroundRemoval(services, (Action<BackgroundRemovalOptions>)null!));
    }
}

