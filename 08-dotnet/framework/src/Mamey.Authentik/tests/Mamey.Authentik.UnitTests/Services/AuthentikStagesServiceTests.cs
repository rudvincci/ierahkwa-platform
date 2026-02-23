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

public class AuthentikStagesServiceTests : TestBase
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AuthentikOptions> _options;
    private readonly ILogger<AuthentikStagesService> _logger;
    private readonly IAuthentikCache _cache;
    private readonly AuthentikStagesService _service;

    public AuthentikStagesServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClientFactory = MockHttpClientFactory.CreateFactory(_mockHttp);
        _options = MockAuthentikOptions.Create();
        _logger = CreateMockLogger<AuthentikStagesService>();
        _cache = new InMemoryAuthentikCache(
            new MemoryCache(new MemoryCacheOptions()),
            CreateMockLogger<InMemoryAuthentikCache>());
        _service = new AuthentikStagesService(_httpClientFactory, _options, _logger, _cache);
    }

    [Fact]
    public void Constructor_WithAllDependencies_CreatesService()
    {
        // Act
        var service = new AuthentikStagesService(_httpClientFactory, _options, _logger, _cache);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task AllListAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { stage_uuid = "uuid-1", name = "Stage 1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/stages/all/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AllListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task AllRetrieveAsync_WithValidUuid_ReturnsStage()
    {
        // Arrange
        var uuid = "stage-uuid-123";
        var expectedStage = new { stage_uuid = uuid, name = "Test Stage" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/stages/all/{uuid}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedStage));

        // Act
        var result = await _service.AllRetrieveAsync(uuid);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task AllTypesListAsync_ReturnsStageTypes()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { name = "AuthenticatorDuoStage", component = "ak-stage-authenticator-duo-form" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/stages/all/types/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AllTypesListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task AllUserSettingsListAsync_ReturnsUserSettings()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { stage = "uuid-1", settings = new { } } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/stages/all/user_settings/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AllUserSettingsListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task AuthenticatorDuoListAsync_WithFilters_ReturnsFilteredStages()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { stage_uuid = "uuid-1", api_hostname = "api.duosecurity.com" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/stages/authenticator/duo/")
            .WithQueryString("api_hostname", "api.duosecurity.com")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AuthenticatorDuoListAsync(api_hostname: "api.duosecurity.com");

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task AuthenticatorDuoCreateAsync_WithValidRequest_CreatesStage()
    {
        // Arrange
        var request = new { name = "Duo Stage", api_hostname = "api.duosecurity.com" };
        var expectedStage = new { stage_uuid = "new-uuid", name = "Duo Stage" };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/stages/authenticator/duo/")
            .With(m => m.Method == HttpMethod.Post)
            .Respond(HttpStatusCode.Created, "application/json", JsonSerializer.Serialize(expectedStage));

        // Act
        var result = await _service.AuthenticatorDuoCreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task AuthenticatorDuoRetrieveAsync_WithValidId_ReturnsStage()
    {
        // Arrange
        var id = 789;
        var expectedStage = new { id = id, name = "Duo Stage" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/stages/authenticator/duo/{id}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedStage));

        // Act
        var result = await _service.AuthenticatorDuoRetrieveAsync(id.ToString());

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }
}
