using System.Net;
using System.Net.Http;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Authentik.Caching;
using Mamey.Authentik.Models;
using Mamey.Authentik.Services;
using Mamey.Authentik.UnitTests.Mocks;
using RichardSzalay.MockHttp;
using Xunit;

namespace Mamey.Authentik.UnitTests.Services;

/// <summary>
/// Tests for additional services (Reports, Tasks, Root, SSF, etc.)
/// </summary>
public class AdditionalServiceTests : TestBase
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AuthentikOptions> _options;
    private readonly IAuthentikCache _cache;

    public AdditionalServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClientFactory = MockHttpClientFactory.CreateFactory(_mockHttp);
        _options = MockAuthentikOptions.Create();
        _cache = new InMemoryAuthentikCache(
            new MemoryCache(new MemoryCacheOptions()),
            CreateMockLogger<InMemoryAuthentikCache>());
    }

    [Fact]
    public void ReportsService_Constructor_CreatesService()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikReportsService>();

        // Act
        var service = new AuthentikReportsService(_httpClientFactory, _options, logger, _cache);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void TasksService_Constructor_CreatesService()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikTasksService>();

        // Act
        var service = new AuthentikTasksService(_httpClientFactory, _options, logger, _cache);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void RootService_Constructor_CreatesService()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikRootService>();

        // Act
        var service = new AuthentikRootService(_httpClientFactory, _options, logger, _cache);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void SsfService_Constructor_CreatesService()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikSsfService>();

        // Act
        var service = new AuthentikSsfService(_httpClientFactory, _options, logger, _cache);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void EnterpriseService_Constructor_CreatesService()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikEnterpriseService>();

        // Act
        var service = new AuthentikEnterpriseService(_httpClientFactory, _options, logger, _cache);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void ManagedService_Constructor_CreatesService()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikManagedService>();

        // Act
        var service = new AuthentikManagedService(_httpClientFactory, _options, logger, _cache);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task ReportsService_GetHttpClient_ReturnsClient()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikReportsService>();
        var service = new AuthentikReportsService(_httpClientFactory, _options, logger, _cache);

        // Act
        var client = service.GetType()
            .GetMethod("GetHttpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(service, null) as HttpClient;

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public async Task TasksService_GetHttpClient_ReturnsClient()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikTasksService>();
        var service = new AuthentikTasksService(_httpClientFactory, _options, logger, _cache);

        // Act
        var client = service.GetType()
            .GetMethod("GetHttpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(service, null) as HttpClient;

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public async Task RootService_GetHttpClient_ReturnsClient()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikRootService>();
        var service = new AuthentikRootService(_httpClientFactory, _options, logger, _cache);

        // Act
        var client = service.GetType()
            .GetMethod("GetHttpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(service, null) as HttpClient;

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public async Task SsfService_GetHttpClient_ReturnsClient()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikSsfService>();
        var service = new AuthentikSsfService(_httpClientFactory, _options, logger, _cache);

        // Act
        var client = service.GetType()
            .GetMethod("GetHttpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(service, null) as HttpClient;

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public async Task EnterpriseService_GetHttpClient_ReturnsClient()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikEnterpriseService>();
        var service = new AuthentikEnterpriseService(_httpClientFactory, _options, logger, _cache);

        // Act
        var client = service.GetType()
            .GetMethod("GetHttpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(service, null) as HttpClient;

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public async Task ManagedService_GetHttpClient_ReturnsClient()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikManagedService>();
        var service = new AuthentikManagedService(_httpClientFactory, _options, logger, _cache);

        // Act
        var client = service.GetType()
            .GetMethod("GetHttpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(service, null) as HttpClient;

        // Assert
        client.Should().NotBeNull();
    }
}
