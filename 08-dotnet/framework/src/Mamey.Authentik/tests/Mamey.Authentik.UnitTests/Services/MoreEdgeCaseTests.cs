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
/// Additional edge case tests for various services.
/// </summary>
public class MoreEdgeCaseTests : TestBase
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AuthentikOptions> _options;
    private readonly ILogger<AuthentikCoreService> _logger;
    private readonly IAuthentikCache _cache;
    private readonly AuthentikCoreService _service;

    public MoreEdgeCaseTests()
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
    public async Task UsersListAsync_WithMaximumPageSize_HandlesCorrectly()
    {
        // Arrange
        var largeResult = new PaginatedResult<object>
        {
            Results = Enumerable.Range(1, 1000).Select(i => new { id = i, username = $"user{i}" }).Cast<object>().ToList(),
            Pagination = new Pagination { Count = 1000 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(largeResult));

        // Act
        var result = await _service.UsersListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1000);
    }

    [Fact]
    public async Task UsersListAsync_WithUnicodeCharacters_HandlesCorrectly()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, username = "用户", email = "用户@example.com" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .With(m => m.RequestUri!.Query.Contains("username="))
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.UsersListAsync(username: "用户");

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task UsersListAsync_WithUrlEncodedCharacters_HandlesCorrectly()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, email = "test%40example.com" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .With(m => m.RequestUri!.Query.Contains("email="))
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.UsersListAsync(email: "test%40example.com");

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task ApplicationsListAsync_WithMultipleBooleanFilters_HandlesAllCombinations()
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
            .WithQueryString("superuser_full_list", "true")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.ApplicationsListAsync(
            only_with_launch_url: true,
            superuser_full_list: true);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task UsersListAsync_WithGroupsFilters_HandlesCorrectly()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, username = "user1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .WithQueryString("groups_by_name", "group1")
            .WithQueryString("groups_by_pk", "1")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.UsersListAsync(
            groups_by_name: "group1",
            groups_by_pk: "1");

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task UsersListAsync_WithTypeFilter_HandlesCorrectly()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, type = "internal" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .WithQueryString("type", "internal")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.UsersListAsync(type: "internal");

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task UsersListAsync_WithAttributesFilter_HandlesCorrectly()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, attributes = new { department = "IT" } } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .WithQueryString("attributes", "department")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.UsersListAsync(attributes: "department");

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task UsersListAsync_WithLastUpdatedFilters_HandlesCorrectly()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, last_updated = "2024-01-01T00:00:00Z" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .WithQueryString("last_updated__gt", "2024-01-01")
            .WithQueryString("last_updated__lt", "2024-12-31")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.UsersListAsync(
            last_updated__gt: "2024-01-01",
            last_updated__lt: "2024-12-31");

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }
}
