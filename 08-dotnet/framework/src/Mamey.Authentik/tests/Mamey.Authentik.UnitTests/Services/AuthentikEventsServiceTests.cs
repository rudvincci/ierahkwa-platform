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

public class AuthentikEventsServiceTests : TestBase
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AuthentikOptions> _options;
    private readonly ILogger<AuthentikEventsService> _logger;
    private readonly IAuthentikCache _cache;
    private readonly AuthentikEventsService _service;

    public AuthentikEventsServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClientFactory = MockHttpClientFactory.CreateFactory(_mockHttp);
        _options = MockAuthentikOptions.Create();
        _logger = CreateMockLogger<AuthentikEventsService>();
        _cache = new InMemoryAuthentikCache(
            new MemoryCache(new MemoryCacheOptions()),
            CreateMockLogger<InMemoryAuthentikCache>());
        _service = new AuthentikEventsService(_httpClientFactory, _options, _logger, _cache);
    }

    [Fact]
    public void Constructor_WithAllDependencies_CreatesService()
    {
        // Act
        var service = new AuthentikEventsService(_httpClientFactory, _options, _logger, _cache);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task EventsListAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { event_uuid = "uuid-1", action = "login" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/events/events/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.EventsListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task EventsListAsync_WithFilters_ReturnsFilteredEvents()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { event_uuid = "uuid-1", action = "login", username = "user1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/events/events/")
            .WithQueryString("action", "login")
            .WithQueryString("username", "user1")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.EventsListAsync(action: "login", username: "user1");

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task EventsRetrieveAsync_WithValidUuid_ReturnsEvent()
    {
        // Arrange
        var uuid = "event-uuid-123";
        var expectedEvent = new { event_uuid = uuid, action = "login", username = "user1" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/events/events/{uuid}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedEvent));

        // Act
        var result = await _service.EventsRetrieveAsync(uuid);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task EventsCreateAsync_WithValidRequest_CreatesEvent()
    {
        // Arrange
        var request = new { action = "custom_action", username = "user1" };
        var expectedEvent = new { event_uuid = "new-uuid", action = "custom_action" };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/events/events/")
            .With(m => m.Method == HttpMethod.Post)
            .Respond(HttpStatusCode.Created, "application/json", JsonSerializer.Serialize(expectedEvent));

        // Act
        var result = await _service.EventsCreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task EventsUpdateAsync_WithValidRequest_UpdatesEvent()
    {
        // Arrange
        var uuid = "event-uuid-123";
        var request = new { action = "updated_action" };
        var expectedEvent = new { event_uuid = uuid, action = "updated_action" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/events/events/{uuid}/")
            .With(m => m.Method == HttpMethod.Put)
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedEvent));

        // Act
        var result = await _service.EventsUpdateAsync(uuid, request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task EventsListAsync_WithMultipleFilters_ReturnsFilteredEvents()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { event_uuid = "uuid-1", action = "login", client_ip = "192.168.1.1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/events/events/")
            .WithQueryString("action", "login")
            .WithQueryString("client_ip", "192.168.1.1")
            .WithQueryString("username", "user1")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.EventsListAsync(
            action: "login",
            client_ip: "192.168.1.1",
            username: "user1");

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }
}
