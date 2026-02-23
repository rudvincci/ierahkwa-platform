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

public class AuthentikOutpostsServiceTests : TestBase
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AuthentikOptions> _options;
    private readonly ILogger<AuthentikOutpostsService> _logger;
    private readonly IAuthentikCache _cache;
    private readonly AuthentikOutpostsService _service;

    public AuthentikOutpostsServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClientFactory = MockHttpClientFactory.CreateFactory(_mockHttp);
        _options = MockAuthentikOptions.Create();
        _logger = CreateMockLogger<AuthentikOutpostsService>();
        _cache = new InMemoryAuthentikCache(
            new MemoryCache(new MemoryCacheOptions()),
            CreateMockLogger<InMemoryAuthentikCache>());
        _service = new AuthentikOutpostsService(_httpClientFactory, _options, _logger, _cache);
    }

    [Fact]
    public void Constructor_WithAllDependencies_CreatesService()
    {
        // Act
        var service = new AuthentikOutpostsService(_httpClientFactory, _options, _logger, _cache);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task InstancesListAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { uuid = "uuid-1", name = "Outpost 1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/outposts/instances/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.InstancesListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task InstancesListAsync_WithFilters_ReturnsFilteredInstances()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { uuid = "uuid-1", name = "Outpost 1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/outposts/instances/")
            .WithQueryString("name__icontains", "test")
            .WithQueryString("providers__isnull", "false")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.InstancesListAsync(name__icontains: "test", providers__isnull: false);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task InstancesRetrieveAsync_WithValidUuid_ReturnsInstance()
    {
        // Arrange
        var uuid = "outpost-uuid-123";
        var expectedInstance = new { uuid = uuid, name = "Test Outpost" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/outposts/instances/{uuid}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedInstance));

        // Act
        var result = await _service.InstancesRetrieveAsync(uuid);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task InstancesCreateAsync_WithValidRequest_CreatesInstance()
    {
        // Arrange
        var request = new { name = "New Outpost", type = "proxy" };
        var expectedInstance = new { uuid = "new-uuid", name = "New Outpost" };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/outposts/instances/")
            .With(m => m.Method == HttpMethod.Post)
            .Respond(HttpStatusCode.Created, "application/json", JsonSerializer.Serialize(expectedInstance));

        // Act
        var result = await _service.InstancesCreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task InstancesUpdateAsync_WithValidRequest_UpdatesInstance()
    {
        // Arrange
        var uuid = "outpost-uuid-123";
        var request = new { name = "Updated Outpost" };
        var expectedInstance = new { uuid = uuid, name = "Updated Outpost" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/outposts/instances/{uuid}/")
            .With(m => m.Method == HttpMethod.Put)
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedInstance));

        // Act
        var result = await _service.InstancesUpdateAsync(uuid, request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task InstancesHealthListAsync_WithValidUuid_ReturnsHealthStatus()
    {
        // Arrange
        var uuid = "outpost-uuid-123";
        var expectedHealth = new { status = "healthy", version = "1.0.0" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/outposts/instances/{uuid}/health/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedHealth));

        // Act
        var result = await _service.InstancesHealthListAsync(uuid);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }
}
