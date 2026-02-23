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

public class AuthentikFlowsServiceTests : TestBase
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AuthentikOptions> _options;
    private readonly ILogger<AuthentikFlowsService> _logger;
    private readonly IAuthentikCache _cache;
    private readonly AuthentikFlowsService _service;

    public AuthentikFlowsServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClientFactory = MockHttpClientFactory.CreateFactory(_mockHttp);
        _options = MockAuthentikOptions.Create();
        _logger = CreateMockLogger<AuthentikFlowsService>();
        _cache = new InMemoryAuthentikCache(
            new MemoryCache(new MemoryCacheOptions()),
            CreateMockLogger<InMemoryAuthentikCache>());
        _service = new AuthentikFlowsService(_httpClientFactory, _options, _logger, _cache);
    }

    [Fact]
    public void Constructor_WithAllDependencies_CreatesService()
    {
        // Act
        var service = new AuthentikFlowsService(_httpClientFactory, _options, _logger, _cache);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task BindingsListAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { fsb_uuid = "uuid-1", target = "target1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/flows/bindings/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.BindingsListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task BindingsListAsync_WithFilters_ReturnsFilteredResults()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { fsb_uuid = "uuid-1", target = "target1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/flows/bindings/")
            .WithQueryString("target", "target1")
            .WithQueryString("evaluate_on_plan", "true")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.BindingsListAsync(target: "target1", evaluate_on_plan: true);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task BindingsRetrieveAsync_WithValidUuid_ReturnsBinding()
    {
        // Arrange
        var uuid = "test-uuid-123";
        var expectedBinding = new { fsb_uuid = uuid, target = "target1", order = 1 };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/flows/bindings/{uuid}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedBinding));

        // Act
        var result = await _service.BindingsRetrieveAsync(uuid);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task BindingsCreateAsync_WithValidRequest_CreatesBinding()
    {
        // Arrange
        var request = new { target = "new-target", order = 1 };
        var expectedBinding = new { fsb_uuid = "new-uuid", target = "new-target", order = 1 };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/flows/bindings/")
            .With(m => m.Method == HttpMethod.Post)
            .Respond(HttpStatusCode.Created, "application/json", JsonSerializer.Serialize(expectedBinding));

        // Act
        var result = await _service.BindingsCreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task BindingsUpdateAsync_WithValidRequest_UpdatesBinding()
    {
        // Arrange
        var uuid = "test-uuid-123";
        var request = new { order = 2 };
        var expectedBinding = new { fsb_uuid = uuid, target = "target1", order = 2 };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/flows/bindings/{uuid}/")
            .With(m => m.Method == HttpMethod.Put)
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedBinding));

        // Act
        var result = await _service.BindingsUpdateAsync(uuid, request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task BindingsPartialUpdateAsync_WithValidRequest_PartiallyUpdatesBinding()
    {
        // Arrange
        var uuid = "test-uuid-123";
        var request = new { order = 3 };
        var expectedBinding = new { fsb_uuid = uuid, target = "target1", order = 3 };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/flows/bindings/{uuid}/")
            .With(m => m.Method == HttpMethod.Patch)
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedBinding));

        // Act
        var result = await _service.BindingsPartialUpdateAsync(uuid, request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task ExecutorGetAsync_WithValidSlug_ReturnsExecutor()
    {
        // Arrange
        var flowSlug = "default-authentication-flow";
        var expectedExecutor = new { flow_slug = flowSlug, components = new[] { "component1" } };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/flows/executor/{flowSlug}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedExecutor));

        // Act
        var result = await _service.ExecutorGetAsync(flowSlug);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task ExecutorGetAsync_WithQuery_ReturnsExecutor()
    {
        // Arrange
        var flowSlug = "default-authentication-flow";
        var query = "?next=/dashboard";
        var expectedExecutor = new { flow_slug = flowSlug, query = query };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/flows/executor/{flowSlug}/")
            .WithQueryString("query", query)
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedExecutor));

        // Act
        var result = await _service.ExecutorGetAsync(flowSlug, query: query);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task InstancesListAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { slug = "flow-1", name = "Flow 1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/flows/instances/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.InstancesListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }
}
