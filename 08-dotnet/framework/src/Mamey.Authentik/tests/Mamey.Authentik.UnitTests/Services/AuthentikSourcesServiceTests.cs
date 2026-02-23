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

public class AuthentikSourcesServiceTests : TestBase
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AuthentikOptions> _options;
    private readonly ILogger<AuthentikSourcesService> _logger;
    private readonly IAuthentikCache _cache;
    private readonly AuthentikSourcesService _service;

    public AuthentikSourcesServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClientFactory = MockHttpClientFactory.CreateFactory(_mockHttp);
        _options = MockAuthentikOptions.Create();
        _logger = CreateMockLogger<AuthentikSourcesService>();
        _cache = new InMemoryAuthentikCache(
            new MemoryCache(new MemoryCacheOptions()),
            CreateMockLogger<InMemoryAuthentikCache>());
        _service = new AuthentikSourcesService(_httpClientFactory, _options, _logger, _cache);
    }

    [Fact]
    public void Constructor_WithAllDependencies_CreatesService()
    {
        // Act
        var service = new AuthentikSourcesService(_httpClientFactory, _options, _logger, _cache);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task AllListAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { slug = "source-1", name = "Source 1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/sources/all/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AllListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task AllRetrieveAsync_WithValidSlug_ReturnsSource()
    {
        // Arrange
        var slug = "test-source";
        var expectedSource = new { slug = slug, name = "Test Source" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/sources/all/{slug}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedSource));

        // Act
        var result = await _service.AllRetrieveAsync(slug);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task AllTypesListAsync_ReturnsSourceTypes()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { name = "LDAPSource", component = "ak-source-ldap-form" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/sources/all/types/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AllTypesListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task LdapListAsync_WithFilters_ReturnsFilteredSources()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { slug = "ldap-1", name = "LDAP Source" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/sources/ldap/")
            .WithQueryString("server_uri", "ldap://example.com")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.LdapListAsync(server_uri: "ldap://example.com");

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task LdapCreateAsync_WithValidRequest_CreatesSource()
    {
        // Arrange
        var request = new { name = "LDAP Source", server_uri = "ldap://example.com" };
        var expectedSource = new { slug = "ldap-1", name = "LDAP Source" };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/sources/ldap/")
            .With(m => m.Method == HttpMethod.Post)
            .Respond(HttpStatusCode.Created, "application/json", JsonSerializer.Serialize(expectedSource));

        // Act
        var result = await _service.LdapCreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task LdapRetrieveAsync_WithValidSlug_ReturnsSource()
    {
        // Arrange
        var slug = "ldap-source";
        var expectedSource = new { slug = slug, name = "LDAP Source" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/sources/ldap/{slug}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedSource));

        // Act
        var result = await _service.LdapRetrieveAsync(slug);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }
}
