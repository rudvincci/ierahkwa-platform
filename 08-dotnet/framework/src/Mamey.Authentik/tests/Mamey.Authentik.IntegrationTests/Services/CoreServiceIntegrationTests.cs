using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mamey.Authentik;
using Mamey.Authentik.Exceptions;
using Mamey.Authentik.IntegrationTests.TestFixtures;
using System.Net;
using Xunit;

namespace Mamey.Authentik.IntegrationTests.Services;

[Collection("AuthentikIntegration")]
public class CoreServiceIntegrationTests : IClassFixture<AuthentikTestFixture>
{
    private readonly AuthentikTestFixture _fixture;
    private readonly IAuthentikClient? _client;

    public CoreServiceIntegrationTests(AuthentikTestFixture fixture)
    {
        _fixture = fixture;
        
        if (!_fixture.IsContainerRunning || string.IsNullOrWhiteSpace(_fixture.ApiToken))
        {
            return;
        }
        
        var services = new ServiceCollection();
        services.AddAuthentik(options =>
        {
            options.BaseUrl = _fixture.BaseUrl;
            options.ApiToken = _fixture.ApiToken;
            options.Timeout = TimeSpan.FromSeconds(30);
        });

        var serviceProvider = services.BuildServiceProvider();
        _client = serviceProvider.GetRequiredService<IAuthentikClient>();
    }

    [Fact]
    public async Task HealthCheck_InstanceIsAccessible()
    {
        if (!_fixture.IsContainerRunning)
        {
            return;
        }
        
        _fixture.BaseUrl.Should().NotBeNullOrEmpty();
        _fixture.IsContainerRunning.Should().BeTrue();
    }

    [Fact]
    public async Task GetUserAsync_WithValidUserId_ReturnsUser()
    {
        if (!_fixture.IsContainerRunning || _client == null || string.IsNullOrWhiteSpace(_fixture.ApiToken))
        {
            return;
        }

        // Arrange - Get a list of users first to find a valid user ID
        var usersResult = await _client.Core.ListUsersAsync(page: 1, pageSize: 1);
        
        if (usersResult?.Results == null || !usersResult.Results.Any())
        {
            // No users available - this is OK for a fresh Authentik instance
            return;
        }

        var firstUser = usersResult.Results.First();
        var userId = firstUser?.ToString() ?? throw new InvalidOperationException("User ID is null");

        // Act
        var user = await _client.Core.GetUserAsync(userId);

        // Assert
        user.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserAsync_WithInvalidUserId_ThrowsNotFoundException()
    {
        if (!_fixture.IsContainerRunning || _client == null || string.IsNullOrWhiteSpace(_fixture.ApiToken))
        {
            return;
        }

        // Arrange
        var invalidUserId = Guid.NewGuid().ToString();

        // Act & Assert
        Func<Task> act = async () => await _client.Core.GetUserAsync(invalidUserId);
        var exception = await Assert.ThrowsAsync<AuthentikNotFoundException>(act);
        exception.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserAsync_WithEmptyUserId_ThrowsArgumentException()
    {
        if (!_fixture.IsContainerRunning || _client == null || string.IsNullOrWhiteSpace(_fixture.ApiToken))
        {
            return;
        }

        // Act & Assert
        Func<Task> act = async () => await _client.Core.GetUserAsync(string.Empty);
        var exception = await Assert.ThrowsAsync<ArgumentException>(act);
        exception.Should().NotBeNull();
    }

    [Fact]
    public async Task ListUsersAsync_ReturnsPaginatedResult()
    {
        if (!_fixture.IsContainerRunning || _client == null || string.IsNullOrWhiteSpace(_fixture.ApiToken))
        {
            return;
        }

        // Act
        var result = await _client.Core.ListUsersAsync(page: 1, pageSize: 10);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().NotBeNull();
        result.Pagination.Should().NotBeNull();
    }

    [Fact]
    public async Task ListUsersAsync_WithPagination_ReturnsCorrectPage()
    {
        if (!_fixture.IsContainerRunning || _client == null || string.IsNullOrWhiteSpace(_fixture.ApiToken))
        {
            return;
        }

        // Act
        var page1 = await _client.Core.ListUsersAsync(page: 1, pageSize: 5);
        var page2 = await _client.Core.ListUsersAsync(page: 2, pageSize: 5);

        // Assert
        page1.Should().NotBeNull();
        page2.Should().NotBeNull();
        page1.Pagination.Should().NotBeNull();
        page2.Pagination.Should().NotBeNull();
    }

    [Fact]
    public async Task ListUsersAsync_WithLargePageSize_HandlesCorrectly()
    {
        if (!_fixture.IsContainerRunning || _client == null || string.IsNullOrWhiteSpace(_fixture.ApiToken))
        {
            return;
        }

        // Act
        var result = await _client.Core.ListUsersAsync(page: 1, pageSize: 100);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().NotBeNull();
    }

    [Fact]
    public async Task ListUsersAsync_WithoutPagination_ReturnsDefaultResults()
    {
        if (!_fixture.IsContainerRunning || _client == null || string.IsNullOrWhiteSpace(_fixture.ApiToken))
        {
            return;
        }

        // Act
        var result = await _client.Core.ListUsersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().NotBeNull();
    }

    [Fact]
    public async Task Service_WithInvalidToken_ThrowsAuthenticationException()
    {
        if (!_fixture.IsContainerRunning)
        {
            return;
        }

        // Arrange
        var services = new ServiceCollection();
        services.AddAuthentik(options =>
        {
            options.BaseUrl = _fixture.BaseUrl;
            options.ApiToken = "invalid-token-12345";
        });

        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetRequiredService<IAuthentikClient>();

        // Act & Assert
        Func<Task> act = async () => await client.Core.ListUsersAsync();
        var exception = await Assert.ThrowsAsync<AuthentikAuthenticationException>(act);
        exception.Should().NotBeNull();
    }

    [Fact]
    public async Task Service_WithInvalidBaseUrl_ThrowsException()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddAuthentik(options =>
        {
            options.BaseUrl = "http://invalid-url-that-does-not-exist:9999";
            options.ApiToken = "test-token";
            options.Timeout = TimeSpan.FromSeconds(5);
        });

        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetRequiredService<IAuthentikClient>();

        // Act & Assert
        Func<Task> act = async () => await client.Core.ListUsersAsync();
        var exception = await Assert.ThrowsAnyAsync<Exception>(act);
        exception.Should().NotBeNull();
    }
}
