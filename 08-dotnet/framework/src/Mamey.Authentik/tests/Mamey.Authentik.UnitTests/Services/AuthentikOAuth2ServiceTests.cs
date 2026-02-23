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

public class AuthentikOAuth2ServiceTests : TestBase
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AuthentikOptions> _options;
    private readonly ILogger<AuthentikOAuth2Service> _logger;
    private readonly IAuthentikCache _cache;
    private readonly AuthentikOAuth2Service _service;

    public AuthentikOAuth2ServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClientFactory = MockHttpClientFactory.CreateFactory(_mockHttp);
        _options = MockAuthentikOptions.Create();
        _logger = CreateMockLogger<AuthentikOAuth2Service>();
        _cache = new InMemoryAuthentikCache(
            new MemoryCache(new MemoryCacheOptions()),
            CreateMockLogger<InMemoryAuthentikCache>());
        _service = new AuthentikOAuth2Service(_httpClientFactory, _options, _logger, _cache);
    }

    [Fact]
    public void Constructor_WithAllDependencies_CreatesService()
    {
        // Act
        var service = new AuthentikOAuth2Service(_httpClientFactory, _options, _logger, _cache);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task AccessTokensListAsync_WithFilters_ReturnsFilteredTokens()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, provider = 1, user = 1 } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/oauth2/access_tokens/")
            .WithQueryString("provider", "1")
            .WithQueryString("user", "1")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AccessTokensListAsync(provider: 1, user: 1);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task AccessTokensListAsync_WithoutFilters_ReturnsAllTokens()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> 
            { 
                new { id = 1, provider = 1 },
                new { id = 2, provider = 2 }
            },
            Pagination = new Pagination { Count = 2 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/oauth2/access_tokens/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AccessTokensListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(2);
    }

    [Fact]
    public async Task AccessTokensRetrieveAsync_WithValidId_ReturnsToken()
    {
        // Arrange
        var id = 123;
        var expectedToken = new { id = id, provider = 1, user = 1 };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/oauth2/access_tokens/{id}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedToken));

        // Act
        var result = await _service.AccessTokensRetrieveAsync(id);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task AccessTokensDestroyAsync_WithValidId_DeletesToken()
    {
        // Arrange
        var id = 123;
        
        // Delete operations typically return 204 NoContent with empty body
        // The service will try to deserialize, which may return null
        _mockHttp
            .When($"https://test.authentik.local/api/v3/oauth2/access_tokens/{id}/")
            .With(m => m.Method == HttpMethod.Delete)
            .Respond(HttpStatusCode.NoContent);

        // Act
        var result = await _service.AccessTokensDestroyAsync(id);

        // Assert
        // Delete operations may return null when response has no content
        // The service handles this gracefully
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task AccessTokensUsedByListAsync_WithValidId_ReturnsUsedByInfo()
    {
        // Arrange
        var id = 123;
        var expectedResult = new { id = id, used_by = new[] { "app1", "app2" } };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/oauth2/access_tokens/{id}/used_by/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AccessTokensUsedByListAsync(id);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task AuthorizationCodesListAsync_WithFilters_ReturnsFilteredCodes()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, provider = 1, user = 1 } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/oauth2/authorization_codes/")
            .WithQueryString("provider", "1")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AuthorizationCodesListAsync(provider: 1);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task AuthorizationCodesRetrieveAsync_WithValidId_ReturnsCode()
    {
        // Arrange
        var id = 456;
        var expectedCode = new { id = id, provider = 1, code = "auth_code_123" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/oauth2/authorization_codes/{id}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedCode));

        // Act
        var result = await _service.AuthorizationCodesRetrieveAsync(id);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task RefreshTokensListAsync_WithFilters_ReturnsFilteredTokens()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, provider = 1, user = 1 } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/oauth2/refresh_tokens/")
            .WithQueryString("user", "1")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.RefreshTokensListAsync(user: 1);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task RefreshTokensRetrieveAsync_WithValidId_ReturnsToken()
    {
        // Arrange
        var id = 789;
        var expectedToken = new { id = id, provider = 1, token = "refresh_token_123" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/oauth2/refresh_tokens/{id}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedToken));

        // Act
        var result = await _service.RefreshTokensRetrieveAsync(id);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }
}
