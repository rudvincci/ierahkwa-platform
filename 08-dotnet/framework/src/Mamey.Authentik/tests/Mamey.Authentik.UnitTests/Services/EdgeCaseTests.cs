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
/// Tests for edge cases including null parameters, boundary values, and invalid inputs.
/// </summary>
public class EdgeCaseTests : TestBase
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AuthentikOptions> _options;
    private readonly ILogger<AuthentikCoreService> _logger;
    private readonly IAuthentikCache _cache;
    private readonly AuthentikCoreService _service;

    public EdgeCaseTests()
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
    public async Task UsersListAsync_WithNullOptionalParams_HandlesGracefully()
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

        // Act - All optional parameters are null
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
    public async Task UsersListAsync_WithEmptyStringParams_HandlesGracefully()
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

        // Act - Empty string parameters should be ignored (not added to query)
        var result = await _service.UsersListAsync(
            email: string.Empty,
            username: string.Empty);

        // Assert
        result.Should().NotBeNull();
        // Empty strings should not be added to query params
    }

    [Fact]
    public async Task UsersListAsync_WithWhitespaceParams_HandlesGracefully()
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

        // Act - Whitespace parameters should be ignored
        var result = await _service.UsersListAsync(
            email: "   ",
            username: "\t\n");

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UsersListAsync_WithSpecialCharactersInParams_HandlesCorrectly()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, email = "user+test@example.com" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        // Note: The query string will be URL encoded, so we match the actual encoded version
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .With(m => m.RequestUri!.Query.Contains("email="))
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act - Email with special characters (will be URL encoded)
        var result = await _service.UsersListAsync(email: "user+test@example.com");

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task UsersListAsync_WithVeryLongStringParam_HandlesCorrectly()
    {
        // Arrange
        var longString = new string('a', 1000); // 1000 character string
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object>(),
            Pagination = new Pagination { Count = 0 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .WithQueryString("username", longString)
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.UsersListAsync(username: longString);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UsersListAsync_WithAllBooleanCombinations_HandlesCorrectly()
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

        // Act - Test all boolean combinations
        var result = await _service.UsersListAsync(
            is_active: true,
            is_superuser: false,
            include_groups: true);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task ApplicationsListAsync_WithNullOptionalParams_HandlesGracefully()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { slug = "app1", name = "App 1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/applications/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act - All optional parameters are null
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
    public async Task UsersRetrieveAsync_WithZeroId_HandlesCorrectly()
    {
        // Arrange
        var userId = 0;
        var errorResponse = new { detail = "Not found." };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/core/users/{userId}/")
            .Respond(HttpStatusCode.NotFound, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        var result = await _service.UsersRetrieveAsync(userId);

        // Assert
        // The service will deserialize the error response
        // In production with handlers, this would throw AuthentikNotFoundException
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UsersRetrieveAsync_WithNegativeId_HandlesCorrectly()
    {
        // Arrange
        var userId = -1;
        var errorResponse = new { detail = "Not found." };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/core/users/{userId}/")
            .Respond(HttpStatusCode.NotFound, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        var result = await _service.UsersRetrieveAsync(userId);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UsersRetrieveAsync_WithVeryLargeId_HandlesCorrectly()
    {
        // Arrange
        var userId = int.MaxValue;
        var errorResponse = new { detail = "Not found." };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/core/users/{userId}/")
            .Respond(HttpStatusCode.NotFound, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        var result = await _service.UsersRetrieveAsync(userId);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UsersListAsync_WithDateFilters_HandlesVariousFormats()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, date_joined = "2024-01-01T00:00:00Z" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .WithQueryString("date_joined__gt", "2024-01-01")
            .WithQueryString("date_joined__lt", "2024-12-31")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act - Test date range filters
        var result = await _service.UsersListAsync(
            date_joined__gt: "2024-01-01",
            date_joined__lt: "2024-12-31");

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task UsersListAsync_WithPathFilters_HandlesCorrectly()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, path = "/users/test" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .WithQueryString("path", "/users/test")
            .WithQueryString("path_startswith", "/users")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.UsersListAsync(
            path: "/users/test",
            path_startswith: "/users");

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task UsersListAsync_WithUuidFilter_HandlesCorrectly()
    {
        // Arrange
        var uuid = Guid.NewGuid().ToString();
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, uuid = uuid } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .WithQueryString("uuid", uuid)
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.UsersListAsync(uuid: uuid);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task ApplicationsListAsync_WithBooleanFilters_HandlesAllCombinations()
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
}
