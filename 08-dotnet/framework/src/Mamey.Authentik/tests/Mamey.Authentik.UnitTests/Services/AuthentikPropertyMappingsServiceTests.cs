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

public class AuthentikPropertyMappingsServiceTests : TestBase
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AuthentikOptions> _options;
    private readonly ILogger<AuthentikPropertyMappingsService> _logger;
    private readonly IAuthentikCache _cache;
    private readonly AuthentikPropertyMappingsService _service;

    public AuthentikPropertyMappingsServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClientFactory = MockHttpClientFactory.CreateFactory(_mockHttp);
        _options = MockAuthentikOptions.Create();
        _logger = CreateMockLogger<AuthentikPropertyMappingsService>();
        _cache = new InMemoryAuthentikCache(
            new MemoryCache(new MemoryCacheOptions()),
            CreateMockLogger<InMemoryAuthentikCache>());
        _service = new AuthentikPropertyMappingsService(_httpClientFactory, _options, _logger, _cache);
    }

    [Fact]
    public void Constructor_WithAllDependencies_CreatesService()
    {
        // Act
        var service = new AuthentikPropertyMappingsService(_httpClientFactory, _options, _logger, _cache);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task AllListAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { pm_uuid = "uuid-1", name = "Mapping 1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/propertymappings/all/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AllListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task AllRetrieveAsync_WithValidUuid_ReturnsMapping()
    {
        // Arrange
        var uuid = "mapping-uuid-123";
        var expectedMapping = new { pm_uuid = uuid, name = "Test Mapping" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/propertymappings/all/{uuid}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedMapping));

        // Act
        var result = await _service.AllRetrieveAsync(uuid);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task AllListAsync_WithFilters_ReturnsFilteredMappings()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { pm_uuid = "uuid-1", name = "Mapping 1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/propertymappings/all/")
            .WithQueryString("managed", "goauthentik.io")
            .WithQueryString("managed__isnull", "false")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AllListAsync(managed: "goauthentik.io", managed__isnull: false);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task AllTestCreateAsync_WithValidRequest_TestsMapping()
    {
        // Arrange
        var uuid = "mapping-uuid-123";
        var request = new { user = 1, context = new { } };
        var expectedResult = new { result = "test-value", messages = new[] { "Test passed" } };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/propertymappings/all/{uuid}/test/")
            .With(m => m.Method == HttpMethod.Post)
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AllTestCreateAsync(uuid, request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task AllTypesListAsync_ReturnsMappingTypes()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { name = "SAMLPropertyMapping", component = "ak-property-mapping-saml-form" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/propertymappings/all/types/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AllTypesListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }
}
