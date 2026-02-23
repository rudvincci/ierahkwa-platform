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

public class AuthentikAuthenticatorsServiceTests : TestBase
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AuthentikOptions> _options;
    private readonly ILogger<AuthentikAuthenticatorsService> _logger;
    private readonly IAuthentikCache _cache;
    private readonly AuthentikAuthenticatorsService _service;

    public AuthentikAuthenticatorsServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClientFactory = MockHttpClientFactory.CreateFactory(_mockHttp);
        _options = MockAuthentikOptions.Create();
        _logger = CreateMockLogger<AuthentikAuthenticatorsService>();
        _cache = new InMemoryAuthentikCache(
            new MemoryCache(new MemoryCacheOptions()),
            CreateMockLogger<InMemoryAuthentikCache>());
        _service = new AuthentikAuthenticatorsService(_httpClientFactory, _options, _logger, _cache);
    }

    [Fact]
    public void Constructor_WithAllDependencies_CreatesService()
    {
        // Act
        var service = new AuthentikAuthenticatorsService(_httpClientFactory, _options, _logger, _cache);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task AdminAllListAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, name = "Authenticator 1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/authenticators/admin/all/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AdminAllListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task AdminAllListAsync_WithUserFilter_ReturnsFilteredResults()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, user = 1, name = "User Authenticator" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/authenticators/admin/all/")
            .WithQueryString("user", "1")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AdminAllListAsync(user: 1);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task AdminDuoListAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, name = "Duo Authenticator" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/authenticators/admin/duo/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AdminDuoListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task AdminDuoRetrieveAsync_WithValidId_ReturnsDuoAuthenticator()
    {
        // Arrange
        var id = 123;
        var expectedDuo = new { id = id, name = "Duo Authenticator" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/authenticators/admin/duo/{id}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedDuo));

        // Act
        var result = await _service.AdminDuoRetrieveAsync(id);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task AdminDuoCreateAsync_WithValidRequest_CreatesDuoAuthenticator()
    {
        // Arrange
        var request = new { name = "New Duo Authenticator" };
        var expectedDuo = new { id = 1, name = "New Duo Authenticator" };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/authenticators/admin/duo/")
            .With(m => m.Method == HttpMethod.Post)
            .Respond(HttpStatusCode.Created, "application/json", JsonSerializer.Serialize(expectedDuo));

        // Act
        var result = await _service.AdminDuoCreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task AdminDuoUpdateAsync_WithValidRequest_UpdatesDuoAuthenticator()
    {
        // Arrange
        var id = 123;
        var request = new { name = "Updated Duo Authenticator" };
        var expectedDuo = new { id = id, name = "Updated Duo Authenticator" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/authenticators/admin/duo/{id}/")
            .With(m => m.Method == HttpMethod.Put)
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedDuo));

        // Act
        var result = await _service.AdminDuoUpdateAsync(id, request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task AdminDuoPartialUpdateAsync_WithValidRequest_PartiallyUpdatesDuoAuthenticator()
    {
        // Arrange
        var id = 123;
        var request = new { name = "Partially Updated Duo" };
        var expectedDuo = new { id = id, name = "Partially Updated Duo" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/authenticators/admin/duo/{id}/")
            .With(m => m.Method == HttpMethod.Patch)
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedDuo));

        // Act
        var result = await _service.AdminDuoPartialUpdateAsync(id, request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task AdminTotpListAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, name = "TOTP Authenticator" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/authenticators/admin/totp/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AdminTotpListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task AdminTotpRetrieveAsync_WithValidId_ReturnsTotpAuthenticator()
    {
        // Arrange
        var id = 456;
        var expectedTotp = new { id = id, name = "TOTP Authenticator" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/authenticators/admin/totp/{id}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedTotp));

        // Act
        var result = await _service.AdminTotpRetrieveAsync(id);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task AdminTotpCreateAsync_WithValidRequest_CreatesTotpAuthenticator()
    {
        // Arrange
        var request = new { name = "New TOTP Authenticator" };
        var expectedTotp = new { id = 1, name = "New TOTP Authenticator" };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/authenticators/admin/totp/")
            .With(m => m.Method == HttpMethod.Post)
            .Respond(HttpStatusCode.Created, "application/json", JsonSerializer.Serialize(expectedTotp));

        // Act
        var result = await _service.AdminTotpCreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task AdminWebauthnListAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, name = "WebAuthn Authenticator" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/authenticators/admin/webauthn/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AdminWebauthnListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task AdminWebauthnRetrieveAsync_WithValidId_ReturnsWebauthnAuthenticator()
    {
        // Arrange
        var id = 789;
        var expectedWebauthn = new { id = id, name = "WebAuthn Authenticator" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/authenticators/admin/webauthn/{id}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedWebauthn));

        // Act
        var result = await _service.AdminWebauthnRetrieveAsync(id);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task AdminStaticListAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, name = "Static Authenticator" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/authenticators/admin/static/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AdminStaticListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task AdminStaticRetrieveAsync_WithValidId_ReturnsStaticAuthenticator()
    {
        // Arrange
        var id = 101;
        var expectedStatic = new { id = id, name = "Static Authenticator" };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/authenticators/admin/static/{id}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedStatic));

        // Act
        var result = await _service.AdminStaticRetrieveAsync(id);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task AdminEmailListAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, name = "Email Authenticator" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/authenticators/admin/email/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AdminEmailListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task AdminSmsListAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { id = 1, name = "SMS Authenticator" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/authenticators/admin/sms/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.AdminSmsListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }
}
