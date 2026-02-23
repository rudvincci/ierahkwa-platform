using System.Net;
using System.Net.Http;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Mamey.Authentik.Caching;
using Mamey.Authentik.Models;
using Mamey.Authentik.Services;
using Mamey.Authentik.UnitTests.Mocks;
using RichardSzalay.MockHttp;
using Xunit;

namespace Mamey.Authentik.UnitTests.Services;

public class AuthentikAdminServiceTests : TestBase
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AuthentikOptions> _options;
    private readonly ILogger<AuthentikAdminService> _logger;
    private readonly IAuthentikCache _cache;
    private readonly AuthentikAdminService _service;

    public AuthentikAdminServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClientFactory = MockHttpClientFactory.CreateFactory(_mockHttp);
        _options = MockAuthentikOptions.Create();
        _logger = CreateMockLogger<AuthentikAdminService>();
        _cache = new InMemoryAuthentikCache(
            new MemoryCache(new MemoryCacheOptions()),
            CreateMockLogger<InMemoryAuthentikCache>());
        _service = new AuthentikAdminService(_httpClientFactory, _options, _logger, _cache);
    }

    [Fact]
    public void Constructor_WithAllDependencies_CreatesService()
    {
        // Act
        var service = new AuthentikAdminService(_httpClientFactory, _options, _logger, _cache);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void GetHttpClient_ReturnsConfiguredClient()
    {
        // Act
        var client = _service.GetType()
            .GetMethod("GetHttpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(_service, null) as HttpClient;

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public async Task AppsListAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { name = "app1" }, new { name = "app2" } },
            Pagination = new Pagination { Count = 2 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/admin/apps/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AppsListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(2);
        result.Pagination.Count.Should().Be(2);
    }

    [Fact]
    public async Task ModelsListAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { name = "model1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/admin/models/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.ModelsListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task SettingsRetrieveAsync_ReturnsSettings()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { key = "setting1", value = "value1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/admin/settings/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.SettingsRetrieveAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task SettingsUpdateAsync_WithValidRequest_UpdatesSettings()
    {
        // Arrange
        var request = new { key = "setting1", value = "newvalue" };
        var expectedResult = new { key = "setting1", value = "newvalue" };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/admin/settings/")
            .With(m => m.Method == HttpMethod.Put)
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.SettingsUpdateAsync(request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task SettingsPartialUpdateAsync_WithValidRequest_PartiallyUpdatesSettings()
    {
        // Arrange
        var request = new { value = "updatedvalue" };
        var expectedResult = new { key = "setting1", value = "updatedvalue" };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/admin/settings/")
            .With(m => m.Method == HttpMethod.Patch)
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.SettingsPartialUpdateAsync(request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task SystemRetrieveAsync_ReturnsSystemInfo()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { version = "1.0.0", status = "ok" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/admin/system/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.SystemRetrieveAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task SystemCreateAsync_CreatesSystemAction()
    {
        // Arrange
        var expectedResult = new { status = "created", message = "System action executed" };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/admin/system/")
            .With(m => m.Method == HttpMethod.Post)
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.SystemCreateAsync();

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task VersionRetrieveAsync_ReturnsVersionInfo()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { version = "2024.1.0", build = "12345" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/admin/version/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.VersionRetrieveAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task VersionHistoryListAsync_WithFilters_ReturnsFilteredHistory()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { version = "2024.1.0", build = "12345" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/admin/version/history/")
            .WithQueryString("version", "2024.1.0")
            .WithQueryString("build", "12345")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.VersionHistoryListAsync(build: "12345", version: "2024.1.0");

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task VersionHistoryListAsync_WithoutFilters_ReturnsAllHistory()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> 
            { 
                new { version = "2024.1.0", build = "12345" },
                new { version = "2024.0.1", build = "12344" }
            },
            Pagination = new Pagination { Count = 2 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/admin/version/history/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.VersionHistoryListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(2);
    }

    [Fact]
    public async Task VersionHistoryRetrieveAsync_WithValidId_ReturnsVersionHistory()
    {
        // Arrange
        var id = 1;
        var expectedResult = new { id = id, version = "2024.1.0", build = "12345" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/admin/version/history/{id}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.VersionHistoryRetrieveAsync(id);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }
}
