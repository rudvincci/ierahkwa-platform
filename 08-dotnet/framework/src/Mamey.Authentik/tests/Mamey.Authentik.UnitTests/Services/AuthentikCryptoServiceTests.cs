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

public class AuthentikCryptoServiceTests : TestBase
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AuthentikOptions> _options;
    private readonly ILogger<AuthentikCryptoService> _logger;
    private readonly IAuthentikCache _cache;
    private readonly AuthentikCryptoService _service;

    public AuthentikCryptoServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClientFactory = MockHttpClientFactory.CreateFactory(_mockHttp);
        _options = MockAuthentikOptions.Create();
        _logger = CreateMockLogger<AuthentikCryptoService>();
        _cache = new InMemoryAuthentikCache(
            new MemoryCache(new MemoryCacheOptions()),
            CreateMockLogger<InMemoryAuthentikCache>());
        _service = new AuthentikCryptoService(_httpClientFactory, _options, _logger, _cache);
    }

    [Fact]
    public void Constructor_WithAllDependencies_CreatesService()
    {
        // Act
        var service = new AuthentikCryptoService(_httpClientFactory, _options, _logger, _cache);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task CertificatekeypairsListAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { name = "cert-1", kp_uuid = "uuid-1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/crypto/certificatekeypairs/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.CertificatekeypairsListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task CertificatekeypairsListAsync_WithFilters_ReturnsFilteredCertificates()
    {
        // Arrange
        var expectedResult = new PaginatedResult<object>
        {
            Results = new List<object> { new { name = "cert-1", kp_uuid = "uuid-1" } },
            Pagination = new Pagination { Count = 1 }
        };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/crypto/certificatekeypairs/")
            .WithQueryString("has_key", "true")
            .WithQueryString("include_details", "true")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedResult));

        // Act
        var result = await _service.CertificatekeypairsListAsync(has_key: true, include_details: true);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task CertificatekeypairsRetrieveAsync_WithValidUuid_ReturnsCertificate()
    {
        // Arrange
        var uuid = "keypair-uuid-123";
        var expectedCert = new { name = "cert-1", kp_uuid = uuid };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/crypto/certificatekeypairs/{uuid}/")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedCert));

        // Act
        var result = await _service.CertificatekeypairsRetrieveAsync(uuid);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task CertificatekeypairsCreateAsync_WithValidRequest_CreatesCertificate()
    {
        // Arrange
        var request = new { name = "New Certificate" };
        var expectedCert = new { name = "New Certificate", kp_uuid = "new-uuid" };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/crypto/certificatekeypairs/")
            .With(m => m.Method == HttpMethod.Post)
            .Respond(HttpStatusCode.Created, "application/json", JsonSerializer.Serialize(expectedCert));

        // Act
        var result = await _service.CertificatekeypairsCreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task CertificatekeypairsUpdateAsync_WithValidRequest_UpdatesCertificate()
    {
        // Arrange
        var uuid = "keypair-uuid-123";
        var request = new { name = "Updated Certificate" };
        var expectedCert = new { name = "Updated Certificate", kp_uuid = uuid };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/crypto/certificatekeypairs/{uuid}/")
            .With(m => m.Method == HttpMethod.Put)
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedCert));

        // Act
        var result = await _service.CertificatekeypairsUpdateAsync(uuid, request);

        // Assert
        result.Should().NotBeNull();
        _mockHttp.VerifyNoOutstandingExpectation();
    }
}
