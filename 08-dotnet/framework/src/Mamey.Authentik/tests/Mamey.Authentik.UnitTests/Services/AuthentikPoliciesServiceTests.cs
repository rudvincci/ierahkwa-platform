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

public class AuthentikPoliciesServiceTests : TestBase
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AuthentikOptions> _options;
    private readonly ILogger<AuthentikPoliciesService> _logger;
    private readonly IAuthentikCache _cache;
    private readonly AuthentikPoliciesService _service;

    public AuthentikPoliciesServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClientFactory = MockHttpClientFactory.CreateFactory(_mockHttp);
        _options = MockAuthentikOptions.Create();
        _logger = CreateMockLogger<AuthentikPoliciesService>();
        _cache = new InMemoryAuthentikCache(
            new MemoryCache(new MemoryCacheOptions()),
            CreateMockLogger<InMemoryAuthentikCache>());
        _service = new AuthentikPoliciesService(_httpClientFactory, _options, _logger, _cache);
    }

    [Fact]
    public void Constructor_WithAllDependencies_CreatesService()
    {
        // Act
        var service = new AuthentikPoliciesService(_httpClientFactory, _options, _logger, _cache);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task AllListAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { policy_uuid = "uuid-1", name = "Policy 1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/policies/all/")
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
            Results = new List<object> { new { policy_uuid = "uuid-1", name = "Policy 1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/policies/all/")
            .WithQueryString("bindings__isnull", "true")
            .WithQueryString("promptstage__isnull", "false")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AllListAsync(bindings__isnull: true, promptstage__isnull: false);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task AllRetrieveAsync_WithValidUuid_ReturnsPolicy()
    {
        // Arrange
        var uuid = "test-policy-uuid";
        var expectedPolicy = new { policy_uuid = uuid, name = "Test Policy" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/policies/all/{uuid}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedPolicy));

        // Act
        var result = await _service.AllRetrieveAsync(uuid);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task AllTestCreateAsync_WithValidRequest_TestsPolicy()
    {
        // Arrange
        var uuid = "test-policy-uuid";
        var request = new { user = 1, context = new { } };
        var expectedResult = new { result = true, messages = new[] { "Policy passed" } };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/policies/all/{uuid}/test/")
            .With(m => m.Method == HttpMethod.Post)
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AllTestCreateAsync(uuid, request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task AllCacheClearCreateAsync_ClearsCache()
    {
        // Arrange
        var expectedResult = new { status = "cleared" };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/policies/all/cache_clear/")
            .With(m => m.Method == HttpMethod.Post)
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AllCacheClearCreateAsync();

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task AllCacheInfoRetrieveAsync_ReturnsCacheInfo()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { cache_size = 100, hit_rate = 0.95 } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/policies/all/cache_info/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AllCacheInfoRetrieveAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task AllTypesListAsync_ReturnsPolicyTypes()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { name = "EventMatcherPolicy", component = "ak-event-matcher-policy-form" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/policies/all/types/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AllTypesListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task BindingsListAsync_WithFilters_ReturnsFilteredBindings()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { policy_binding_uuid = "uuid-1", target = "target1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/policies/bindings/")
            .WithQueryString("enabled", "true")
            .WithQueryString("order", "1")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.BindingsListAsync(enabled: true, order: 1);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task BindingsCreateAsync_WithValidRequest_CreatesBinding()
    {
        // Arrange
        var request = new { target = "target1", order = 1 };
        var expectedBinding = new { policy_binding_uuid = "new-uuid", target = "target1", order = 1 };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/policies/bindings/")
            .With(m => m.Method == HttpMethod.Post)
            .Respond(HttpStatusCode.Created, "application/json", JsonSerializer.Serialize(expectedBinding));

        // Act
        var result = await _service.BindingsCreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task BindingsRetrieveAsync_WithValidUuid_ReturnsBinding()
    {
        // Arrange
        var uuid = "binding-uuid-123";
        var expectedBinding = new { policy_binding_uuid = uuid, target = "target1" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/policies/bindings/{uuid}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedBinding));

        // Act
        var result = await _service.BindingsRetrieveAsync(uuid);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }
}
