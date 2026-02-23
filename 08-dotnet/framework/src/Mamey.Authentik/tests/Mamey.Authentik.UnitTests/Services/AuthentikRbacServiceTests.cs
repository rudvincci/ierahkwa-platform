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

public class AuthentikRbacServiceTests : TestBase
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AuthentikOptions> _options;
    private readonly ILogger<AuthentikRbacService> _logger;
    private readonly IAuthentikCache _cache;
    private readonly AuthentikRbacService _service;

    public AuthentikRbacServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClientFactory = MockHttpClientFactory.CreateFactory(_mockHttp);
        _options = MockAuthentikOptions.Create();
        _logger = CreateMockLogger<AuthentikRbacService>();
        _cache = new InMemoryAuthentikCache(
            new MemoryCache(new MemoryCacheOptions()),
            CreateMockLogger<InMemoryAuthentikCache>());
        _service = new AuthentikRbacService(_httpClientFactory, _options, _logger, _cache);
    }

    [Fact]
    public void Constructor_WithAllDependencies_CreatesService()
    {
        // Act
        var service = new AuthentikRbacService(_httpClientFactory, _options, _logger, _cache);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task InitialPermissionsListAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, name = "Permission 1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/rbac/initial_permissions/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.InitialPermissionsListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task InitialPermissionsRetrieveAsync_WithValidId_ReturnsPermission()
    {
        // Arrange
        var id = 123;
        var expectedPermission = new { id = id, name = "Test Permission" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/rbac/initial_permissions/{id}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedPermission));

        // Act
        var result = await _service.InitialPermissionsRetrieveAsync(id);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task InitialPermissionsCreateAsync_WithValidRequest_CreatesPermission()
    {
        // Arrange
        var request = new { name = "New Permission" };
        var expectedPermission = new { id = 1, name = "New Permission" };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/rbac/initial_permissions/")
            .With(m => m.Method == HttpMethod.Post)
            .Respond(HttpStatusCode.Created, "application/json", JsonSerializer.Serialize(expectedPermission));

        // Act
        var result = await _service.InitialPermissionsCreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task PermissionsListAsync_WithFilters_ReturnsFilteredPermissions()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, codename = "view_user" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/rbac/permissions/")
            .WithQueryString("codename", "view_user")
            .WithQueryString("user", "1")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.PermissionsListAsync(codename: "view_user", user: 1);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task RolesListAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, name = "Admin Role" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/rbac/roles/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.RolesListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task RolesRetrieveAsync_WithValidId_ReturnsRole()
    {
        // Arrange
        var id = "role-uuid-456";
        var expectedRole = new { id = id, name = "Test Role" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/rbac/roles/{id}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedRole));

        // Act
        var result = await _service.RolesRetrieveAsync(id);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }
}
