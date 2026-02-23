using System.Net;
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
/// Additional boundary value and invalid input tests.
/// </summary>
public class AdditionalBoundaryTests : TestBase
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AuthentikOptions> _options;
    private readonly ILogger<AuthentikCoreService> _logger;
    private readonly IAuthentikCache _cache;
    private readonly AuthentikCoreService _service;

    public AdditionalBoundaryTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClientFactory = MockHttpClientFactory.CreateFactory(_mockHttp);
        _options = MockAuthentikOptions.Create();
        _logger = CreateMockLogger<AuthentikCoreService>();
        _cache = new InMemoryAuthentikCache(
            new MemoryCache(new MemoryCacheOptions()),
            CreateMockLogger<InMemoryAuthentikCache>());
        _service = new AuthentikCoreService(_httpClientFactory, _options, _logger, _cache);
    }

    [Fact]
    public async Task UsersListAsync_WithAllNullFilters_HandlesCorrectly()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, username = "user1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act - All filters null
        var result = await _service.UsersListAsync(
            attributes: null,
            date_joined: null,
            email: null,
            username: null,
            is_active: null,
            is_superuser: null);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task UsersListAsync_WithAllBooleanFilters_HandlesCorrectly()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, is_active = true, is_superuser = false } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .WithQueryString("is_active", "true")
            .WithQueryString("is_superuser", "false")
            .WithQueryString("include_groups", "true")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.UsersListAsync(
            is_active: true,
            is_superuser: false,
            include_groups: true);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task UsersListAsync_WithComplexFilterCombination_HandlesCorrectly()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, username = "user1", email = "user1@example.com" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .WithQueryString("email", "user1@example.com")
            .WithQueryString("is_active", "true")
            .WithQueryString("include_groups", "true")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.UsersListAsync(
            email: "user1@example.com",
            is_active: true,
            include_groups: true);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task ApplicationsListAsync_WithAllNullParams_HandlesCorrectly()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { slug = "app1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/applications/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.ApplicationsListAsync(
            for_user: null,
            group: null,
            slug: null,
            only_with_launch_url: null,
            superuser_full_list: null);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task ApplicationsListAsync_WithBooleanFilters_HandlesCorrectly()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { slug = "app1", only_with_launch_url = true } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/applications/")
            .WithQueryString("only_with_launch_url", "true")
            .WithQueryString("superuser_full_list", "false")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.ApplicationsListAsync(
            only_with_launch_url: true,
            superuser_full_list: false);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task UsersListAsync_WithEmptyEmail_HandlesCorrectly()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object>(),
            Pagination = new Pagination { Count = 0 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act - Empty string should be handled gracefully
        var result = await _service.UsersListAsync(email: string.Empty);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UsersListAsync_WithEmptyUsername_HandlesCorrectly()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object>(),
            Pagination = new Pagination { Count = 0 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act - Empty string should be handled gracefully
        var result = await _service.UsersListAsync(username: string.Empty);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UsersListAsync_WithNullEmailAndUsername_HandlesCorrectly()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, username = "user1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act - Both null should work fine
        var result = await _service.UsersListAsync(email: null, username: null);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }
}
