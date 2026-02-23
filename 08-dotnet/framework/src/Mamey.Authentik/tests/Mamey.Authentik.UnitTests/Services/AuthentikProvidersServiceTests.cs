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

public class AuthentikProvidersServiceTests : TestBase
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AuthentikOptions> _options;
    private readonly ILogger<AuthentikProvidersService> _logger;
    private readonly IAuthentikCache _cache;
    private readonly AuthentikProvidersService _service;

    public AuthentikProvidersServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClientFactory = MockHttpClientFactory.CreateFactory(_mockHttp);
        _options = MockAuthentikOptions.Create();
        _logger = CreateMockLogger<AuthentikProvidersService>();
        _cache = new InMemoryAuthentikCache(
            new MemoryCache(new MemoryCacheOptions()),
            CreateMockLogger<InMemoryAuthentikCache>());
        _service = new AuthentikProvidersService(_httpClientFactory, _options, _logger, _cache);
    }

    [Fact]
    public void Constructor_WithAllDependencies_CreatesService()
    {
        // Act
        var service = new AuthentikProvidersService(_httpClientFactory, _options, _logger, _cache);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task AllListAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, name = "Provider 1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/providers/all/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AllListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task AllListAsync_WithFilters_ReturnsFilteredResults()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, name = "Provider 1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/providers/all/")
            .WithQueryString("application__isnull", "true")
            .WithQueryString("backchannel", "false")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AllListAsync(application__isnull: true, backchannel: false);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task AllRetrieveAsync_WithValidId_ReturnsProvider()
    {
        // Arrange
        var id = 123;
        var expectedProvider = new { id = id, name = "Test Provider" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/providers/all/{id}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedProvider));

        // Act
        var result = await _service.AllRetrieveAsync(id);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task AllTypesListAsync_ReturnsProviderTypes()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { name = "OAuth2Provider", component = "ak-provider-oauth2-form" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/providers/all/types/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AllTypesListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task GoogleWorkspaceListAsync_WithFilters_ReturnsFilteredProviders()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, name = "Google Workspace Provider" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/providers/google_workspace/")
            .WithQueryString("delegated_subject", "subject@example.com")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.GoogleWorkspaceListAsync(delegated_subject: "subject@example.com");

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task GoogleWorkspaceCreateAsync_WithValidRequest_CreatesProvider()
    {
        // Arrange
        var request = new { name = "New Google Workspace Provider" };
        var expectedProvider = new { id = 1, name = "New Google Workspace Provider" };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/providers/google_workspace/")
            .With(m => m.Method == HttpMethod.Post)
            .Respond(HttpStatusCode.Created, "application/json", JsonSerializer.Serialize(expectedProvider));

        // Act
        var result = await _service.GoogleWorkspaceCreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GoogleWorkspaceRetrieveAsync_WithValidId_ReturnsProvider()
    {
        // Arrange
        var id = 456;
        var expectedProvider = new { id = id, name = "Google Workspace Provider" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/providers/google_workspace/{id}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedProvider));

        // Act
        var result = await _service.GoogleWorkspaceRetrieveAsync(id);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }
}
