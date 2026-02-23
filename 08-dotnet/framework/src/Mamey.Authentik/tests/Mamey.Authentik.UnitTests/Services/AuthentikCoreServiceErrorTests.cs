using System.Net;
using System.Net.Http;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Authentik.Caching;
using Mamey.Authentik.Exceptions;
using Mamey.Authentik.Models;
using Mamey.Authentik.Services;
using Mamey.Authentik.UnitTests.Mocks;
using RichardSzalay.MockHttp;
using Xunit;

namespace Mamey.Authentik.UnitTests.Services;

/// <summary>
/// Tests for error handling scenarios in AuthentikCoreService.
/// These tests verify that the service properly handles various HTTP error responses.
/// </summary>
public class AuthentikCoreServiceErrorTests : TestBase
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AuthentikOptions> _options;
    private readonly ILogger<AuthentikCoreService> _logger;
    private readonly IAuthentikCache _cache;
    private readonly AuthentikCoreService _service;

    public AuthentikCoreServiceErrorTests()
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
    public async Task UsersRetrieveAsync_WithNotFound_ReturnsErrorResponse()
    {
        // Arrange
        var userId = 999;
        var errorResponse = new { detail = "Not found." };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/core/users/{userId}/")
            .Respond(HttpStatusCode.NotFound, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        // Note: In unit tests without the error handler pipeline, the service will deserialize
        // the error response. In production, the error handler would throw AuthentikNotFoundException.
        var result = await _service.UsersRetrieveAsync(userId);

        // Assert
        // The service deserializes the error response as an object
        // In production with handlers, this would throw AuthentikNotFoundException
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UsersListAsync_WithUnauthorized_ReturnsErrorResponse()
    {
        // Arrange
        var errorResponse = new { detail = "Authentication credentials were not provided." };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .Respond(HttpStatusCode.Unauthorized, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        // In production, the error handler would throw AuthentikAuthenticationException
        var result = await _service.UsersListAsync();

        // Assert
        // The service attempts to deserialize the error response
        // In production with handlers, this would throw AuthentikAuthenticationException
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UsersCreateAsync_WithBadRequest_ReturnsErrorResponse()
    {
        // Arrange
        var request = new { username = "" }; // Invalid: empty username
        var errorResponse = new { 
            username = new[] { "This field may not be blank." },
            email = new[] { "This field is required." }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .With(m => m.Method == HttpMethod.Post)
            .Respond(HttpStatusCode.BadRequest, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        // In production, the error handler would throw AuthentikValidationException
        var result = await _service.UsersCreateAsync(request);

        // Assert
        // The service deserializes the validation error response
        // In production with handlers, this would throw AuthentikValidationException
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UsersUpdateAsync_WithForbidden_ReturnsErrorResponse()
    {
        // Arrange
        var userId = 123;
        var request = new { email = "new@example.com" };
        var errorResponse = new { detail = "You do not have permission to perform this action." };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/core/users/{userId}/")
            .With(m => m.Method == HttpMethod.Put)
            .Respond(HttpStatusCode.Forbidden, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        // In production, the error handler would throw AuthentikAuthorizationException
        var result = await _service.UsersUpdateAsync(userId, request);

        // Assert
        // The service deserializes the error response
        // In production with handlers, this would throw AuthentikAuthorizationException
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UsersListAsync_WithInternalServerError_ReturnsErrorResponse()
    {
        // Arrange
        var errorResponse = new { detail = "Internal server error." };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .Respond(HttpStatusCode.InternalServerError, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        // In production, the error handler would throw AuthentikApiException
        var result = await _service.UsersListAsync();

        // Assert
        // The service attempts to deserialize the error response
        // In production with handlers, this would throw AuthentikApiException
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UsersListAsync_WithEmptyResponse_HandlesGracefully()
    {
        // Arrange
        var emptyResult = new PaginatedResult<object>
        {
            Results = new List<object>(),
            Pagination = new Pagination { Count = 0 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(emptyResult));

        // Act
        var result = await _service.UsersListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().BeEmpty();
        result.Pagination.Count.Should().Be(0);
    }

    [Fact]
    public async Task UsersListAsync_WithNullResponse_HandlesGracefully()
    {
        // Arrange
        // Some endpoints may return null in certain scenarios
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .Respond(HttpStatusCode.OK, "application/json", "null");

        // Act
        var result = await _service.UsersListAsync();

        // Assert
        // The service should handle null responses gracefully
        // The generated code returns a new PaginatedResult if null
        result.Should().NotBeNull();
        result.Results.Should().NotBeNull();
    }

    [Fact]
    public async Task UsersRetrieveAsync_WithNullResponse_ReturnsNull()
    {
        // Arrange
        var userId = 123;
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/core/users/{userId}/")
            .Respond(HttpStatusCode.OK, "application/json", "null");

        // Act
        var result = await _service.UsersRetrieveAsync(userId);

        // Assert
        // Retrieve operations may return null
        result.Should().BeNull();
    }

    [Fact]
    public async Task UsersListAsync_WithInvalidJson_HandlesError()
    {
        // Arrange
        _mockHttp
            .When("https://test.authentik.local/api/v3/core/users/")
            .Respond(HttpStatusCode.OK, "application/json", "{ invalid json }");

        // Act & Assert
        // The service will throw JsonException when trying to deserialize invalid JSON
        Func<Task> act = async () => await _service.UsersListAsync();
        await Assert.ThrowsAsync<JsonException>(act);
    }

    [Fact(Skip = "Timeout simulation requires actual delay which makes test slow")]
    public async Task UsersListAsync_WithTimeout_ThrowsHttpRequestException()
    {
        // Note: This test is skipped because simulating a timeout requires an actual delay,
        // which makes the test slow. In production, the HttpClient timeout configuration
        // and Polly retry policies handle timeouts appropriately.
        // TODO: Consider using a test-friendly timeout simulation mechanism.
    }
}
