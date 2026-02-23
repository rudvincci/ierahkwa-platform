using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Authentik.Caching;
using Mamey.Authentik.Exceptions;
using Mamey.Authentik.Services;
using Mamey.Authentik.UnitTests.Mocks;
using RichardSzalay.MockHttp;
using Xunit;

namespace Mamey.Authentik.UnitTests.Services;

/// <summary>
/// Additional error path tests for various services.
/// </summary>
public class AdditionalErrorPathTests : TestBase
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AuthentikOptions> _options;
    private readonly IAuthentikCache _cache;

    public AdditionalErrorPathTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClientFactory = MockHttpClientFactory.CreateFactory(_mockHttp);
        _options = MockAuthentikOptions.Create();
        _cache = new InMemoryAuthentikCache(
            new MemoryCache(new MemoryCacheOptions()),
            CreateMockLogger<InMemoryAuthentikCache>());
    }

    [Fact]
    public async Task PoliciesService_AllRetrieveAsync_With404_HandlesNotFound()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikPoliciesService>();
        var service = new AuthentikPoliciesService(_httpClientFactory, _options, logger, _cache);
        var uuid = "non-existent-uuid";
        var errorResponse = new { detail = "Not found." };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/policies/all/{uuid}/")
            .Respond(HttpStatusCode.NotFound, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        var result = await service.AllRetrieveAsync(uuid);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task ProvidersService_AllRetrieveAsync_With404_HandlesNotFound()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikProvidersService>();
        var service = new AuthentikProvidersService(_httpClientFactory, _options, logger, _cache);
        var id = 99999;
        var errorResponse = new { detail = "Not found." };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/providers/all/{id}/")
            .Respond(HttpStatusCode.NotFound, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        var result = await service.AllRetrieveAsync(id);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task StagesService_AllRetrieveAsync_With404_HandlesNotFound()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikStagesService>();
        var service = new AuthentikStagesService(_httpClientFactory, _options, logger, _cache);
        var uuid = "non-existent-stage-uuid";
        var errorResponse = new { detail = "Not found." };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/stages/all/{uuid}/")
            .Respond(HttpStatusCode.NotFound, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        var result = await service.AllRetrieveAsync(uuid);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task SourcesService_AllRetrieveAsync_With404_HandlesNotFound()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikSourcesService>();
        var service = new AuthentikSourcesService(_httpClientFactory, _options, logger, _cache);
        var slug = "non-existent-source";
        var errorResponse = new { detail = "Not found." };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/sources/all/{slug}/")
            .Respond(HttpStatusCode.NotFound, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        var result = await service.AllRetrieveAsync(slug);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task EventsService_EventsRetrieveAsync_With404_HandlesNotFound()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikEventsService>();
        var service = new AuthentikEventsService(_httpClientFactory, _options, logger, _cache);
        var uuid = "non-existent-event-uuid";
        var errorResponse = new { detail = "Not found." };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/events/events/{uuid}/")
            .Respond(HttpStatusCode.NotFound, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        var result = await service.EventsRetrieveAsync(uuid);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task RbacService_InitialPermissionsRetrieveAsync_With404_HandlesNotFound()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikRbacService>();
        var service = new AuthentikRbacService(_httpClientFactory, _options, logger, _cache);
        var id = 99999;
        var errorResponse = new { detail = "Not found." };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/rbac/initial_permissions/{id}/")
            .Respond(HttpStatusCode.NotFound, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        var result = await service.InitialPermissionsRetrieveAsync(id);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task OutpostsService_InstancesRetrieveAsync_With404_HandlesNotFound()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikOutpostsService>();
        var service = new AuthentikOutpostsService(_httpClientFactory, _options, logger, _cache);
        var uuid = "non-existent-outpost-uuid";
        var errorResponse = new { detail = "Not found." };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/outposts/instances/{uuid}/")
            .Respond(HttpStatusCode.NotFound, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        var result = await service.InstancesRetrieveAsync(uuid);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CryptoService_CertificatekeypairsRetrieveAsync_With404_HandlesNotFound()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikCryptoService>();
        var service = new AuthentikCryptoService(_httpClientFactory, _options, logger, _cache);
        var uuid = "non-existent-cert-uuid";
        var errorResponse = new { detail = "Not found." };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/crypto/certificatekeypairs/{uuid}/")
            .Respond(HttpStatusCode.NotFound, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        var result = await service.CertificatekeypairsRetrieveAsync(uuid);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task AuthenticatorsService_AdminDuoRetrieveAsync_With404_HandlesNotFound()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikAuthenticatorsService>();
        var service = new AuthentikAuthenticatorsService(_httpClientFactory, _options, logger, _cache);
        var id = 99999;
        var errorResponse = new { detail = "Not found." };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/authenticators/admin/duo/{id}/")
            .Respond(HttpStatusCode.NotFound, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        var result = await service.AdminDuoRetrieveAsync(id);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task PropertyMappingsService_AllRetrieveAsync_With404_HandlesNotFound()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikPropertyMappingsService>();
        var service = new AuthentikPropertyMappingsService(_httpClientFactory, _options, logger, _cache);
        var uuid = "non-existent-mapping-uuid";
        var errorResponse = new { detail = "Not found." };
        
        _mockHttp
            .When($"https://test.authentik.local/api/v3/propertymappings/all/{uuid}/")
            .Respond(HttpStatusCode.NotFound, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        var result = await service.AllRetrieveAsync(uuid);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task PoliciesService_BindingsCreateAsync_With400_HandlesBadRequest()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikPoliciesService>();
        var service = new AuthentikPoliciesService(_httpClientFactory, _options, logger, _cache);
        var request = new { invalid_field = "invalid" };
        var errorResponse = new { detail = "Invalid request.", field_errors = new { name = new[] { "This field is required." } } };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/policies/bindings/")
            .With(m => m.Method == System.Net.Http.HttpMethod.Post)
            .Respond(HttpStatusCode.BadRequest, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        var result = await service.BindingsCreateAsync(request);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task ProvidersService_GoogleWorkspaceCreateAsync_With400_HandlesBadRequest()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikProvidersService>();
        var service = new AuthentikProvidersService(_httpClientFactory, _options, logger, _cache);
        var request = new { invalid_field = "invalid" };
        var errorResponse = new { detail = "Invalid request." };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/providers/google_workspace/")
            .With(m => m.Method == System.Net.Http.HttpMethod.Post)
            .Respond(HttpStatusCode.BadRequest, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        var result = await service.GoogleWorkspaceCreateAsync(request);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task StagesService_AuthenticatorDuoCreateAsync_With400_HandlesBadRequest()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikStagesService>();
        var service = new AuthentikStagesService(_httpClientFactory, _options, logger, _cache);
        var request = new { invalid_field = "invalid" };
        var errorResponse = new { detail = "Invalid request." };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/stages/authenticator/duo/")
            .With(m => m.Method == System.Net.Http.HttpMethod.Post)
            .Respond(HttpStatusCode.BadRequest, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        var result = await service.AuthenticatorDuoCreateAsync(request);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task SourcesService_LdapCreateAsync_With400_HandlesBadRequest()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikSourcesService>();
        var service = new AuthentikSourcesService(_httpClientFactory, _options, logger, _cache);
        var request = new { invalid_field = "invalid" };
        var errorResponse = new { detail = "Invalid request." };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/sources/ldap/")
            .With(m => m.Method == System.Net.Http.HttpMethod.Post)
            .Respond(HttpStatusCode.BadRequest, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        var result = await service.LdapCreateAsync(request);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task EventsService_EventsCreateAsync_With400_HandlesBadRequest()
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikEventsService>();
        var service = new AuthentikEventsService(_httpClientFactory, _options, logger, _cache);
        var request = new { invalid_field = "invalid" };
        var errorResponse = new { detail = "Invalid request." };
        
        _mockHttp
            .When("https://test.authentik.local/api/v3/events/events/")
            .With(m => m.Method == System.Net.Http.HttpMethod.Post)
            .Respond(HttpStatusCode.BadRequest, "application/json", JsonSerializer.Serialize(errorResponse));

        // Act
        var result = await service.EventsCreateAsync(request);

        // Assert
        result.Should().NotBeNull();
    }
}
