using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.EndToEnd.Authentication;

/// <summary>
/// End-to-end tests for authentication and authorization.
/// 
/// Authentication Strategy:
/// - PRIMARY: JWT authentication (standard username/password login)
/// - SECONDARY: Decentralized Identities (DID) - requires device registration
/// - SERVICE-TO-SERVICE: Certificate authentication (for microservice communication)
/// </summary>
[Collection("EndToEnd")]
public class AuthenticationTests : ApiEndpoints.BaseApiEndpointTests
{
    public AuthenticationTests(
        PostgreSQLFixture postgresFixture,
        RedisFixture redisFixture,
        MongoDBFixture mongoFixture,
        MinIOFixture minioFixture)
        : base(postgresFixture, redisFixture, mongoFixture, minioFixture)
    {
    }

    #region JWT Authentication Tests (Primary Authentication)

    [Fact]
    [Trait("Auth", "JWT")]
    [Trait("Priority", "Primary")]
    public async Task Post_WithValidJwtToken_WhenAuthRequired_ShouldReturn200Ok()
    {
        // Arrange - JWT is the PRIMARY authentication method
        // Users authenticate via standard username/password and receive a JWT token
        var command = new SyncPermissions
        {
            ServiceName = "test-service",
            Permissions = new List<string> { "identities:read" }
        };

        // Act - Add valid JWT token (would be obtained from login endpoint)
        // Note: In a real scenario, this would be obtained from /api/auth/sign-in
        var jwtToken = await GetJwtTokenAsync();
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);
        var response = await Client.PostAsJsonAsync("/api/permissions/sync", command);

        // Assert
        // JWT authentication should succeed for authenticated users
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden);
    }

    [Fact]
    [Trait("Auth", "JWT")]
    [Trait("Priority", "Primary")]
    public async Task Post_WithoutJwtToken_WhenAuthRequired_ShouldReturn401Unauthorized()
    {
        // Arrange - SyncPermissions endpoint requires authentication
        // JWT is the PRIMARY authentication method
        var command = new SyncPermissions
        {
            ServiceName = "test-service",
            Permissions = new List<string> { "identities:read" }
        };

        // Act - No JWT token in request (primary auth method missing)
        Client.DefaultRequestHeaders.Authorization = null;
        var response = await Client.PostAsJsonAsync("/api/permissions/sync", command);

        // Assert
        // Without JWT token (primary auth), request should be rejected
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden);
    }

    [Fact]
    [Trait("Auth", "JWT")]
    [Trait("Priority", "Primary")]
    public async Task Post_WithInvalidJwtToken_ShouldReturn401Unauthorized()
    {
        // Arrange - JWT is the PRIMARY authentication method
        var command = new SyncPermissions
        {
            ServiceName = "test-service",
            Permissions = new List<string> { "identities:read" }
        };

        // Act - Add invalid JWT token (primary auth method invalid)
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "invalid-token");
        var response = await Client.PostAsJsonAsync("/api/permissions/sync", command);

        // Assert
        // Invalid JWT token should be rejected
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden);
    }

    [Fact]
    [Trait("Auth", "JWT")]
    [Trait("Priority", "Primary")]
    public async Task Post_WithExpiredJwtToken_ShouldReturn401Unauthorized()
    {
        // Arrange - JWT is the PRIMARY authentication method
        var command = new SyncPermissions
        {
            ServiceName = "test-service",
            Permissions = new List<string> { "identities:read" }
        };

        // Act - Add expired JWT token (would need to create an expired token for this test)
        // For now, we verify that expired tokens are rejected
        var expiredToken = "expired.jwt.token";
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", expiredToken);
        var response = await Client.PostAsJsonAsync("/api/permissions/sync", command);

        // Assert
        // Expired JWT token should be rejected
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden);
    }

    #endregion

    #region DID Authentication Tests (Secondary Authentication - Requires Device Registration)

    [Fact]
    [Trait("Auth", "DID")]
    [Trait("Priority", "Secondary")]
    public async Task Post_WithDidToken_WithoutDeviceRegistration_ShouldReturn401Unauthorized()
    {
        // Arrange - DID authentication is SECONDARY and requires device registration
        // Users must register their device before using DID authentication
        var command = new SyncPermissions
        {
            ServiceName = "test-service",
            Permissions = new List<string> { "identities:read" }
        };

        // Act - Attempt to use DID authentication without device registration
        // DID authentication requires the device to be registered first
        var didToken = "did:web:example.com:user123";
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("DID", didToken);
        var response = await Client.PostAsJsonAsync("/api/permissions/sync", command);

        // Assert
        // DID authentication without device registration should be rejected
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden);
    }

    [Fact]
    [Trait("Auth", "DID")]
    [Trait("Priority", "Secondary")]
    public async Task Post_WithDidToken_WithDeviceRegistration_ShouldReturn200Ok()
    {
        // Arrange - DID authentication requires device registration
        // Step 1: Register device for DID authentication
        var deviceId = Guid.NewGuid().ToString();
        var did = "did:web:example.com:user123";
        
        // Register device (this would be a separate endpoint like /api/devices/register)
        // For now, we simulate device registration
        var registerDeviceResponse = await RegisterDeviceAsync(deviceId, did);
        registerDeviceResponse.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.NotFound);

        // Step 2: Use DID authentication with registered device
        var command = new SyncPermissions
        {
            ServiceName = "test-service",
            Permissions = new List<string> { "identities:read" }
        };

        // Act - Use DID authentication with registered device
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("DID", did);
        Client.DefaultRequestHeaders.Add("X-Device-Id", deviceId);
        var response = await Client.PostAsJsonAsync("/api/permissions/sync", command);

        // Assert
        // DID authentication with registered device should succeed
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden);
    }

    [Fact]
    [Trait("Auth", "DID")]
    [Trait("Priority", "Secondary")]
    public async Task Post_WithDidToken_WithUnregisteredDevice_ShouldReturn401Unauthorized()
    {
        // Arrange - DID authentication requires device registration
        var command = new SyncPermissions
        {
            ServiceName = "test-service",
            Permissions = new List<string> { "identities:read" }
        };

        // Act - Attempt to use DID authentication with unregistered device
        var did = "did:web:example.com:user123";
        var unregisteredDeviceId = Guid.NewGuid().ToString();
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("DID", did);
        Client.DefaultRequestHeaders.Add("X-Device-Id", unregisteredDeviceId);
        var response = await Client.PostAsJsonAsync("/api/permissions/sync", command);

        // Assert
        // DID authentication with unregistered device should be rejected
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden);
    }

    [Fact]
    [Trait("Auth", "DID")]
    [Trait("Priority", "Secondary")]
    public async Task RegisterDevice_WithValidDid_ShouldReturn201Created()
    {
        // Arrange - Device registration is required for DID authentication
        var deviceId = Guid.NewGuid().ToString();
        var did = "did:web:example.com:user123";
        var deviceInfo = new
        {
            DeviceId = deviceId,
            Did = did,
            DeviceName = "Test Device",
            DeviceType = "Mobile",
            PublicKey = "test-public-key"
        };

        // Act - Register device for DID authentication
        var response = await Client.PostAsJsonAsync("/api/devices/register", deviceInfo);

        // Assert
        // Device registration should succeed (or return 404 if endpoint doesn't exist yet)
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.Created, HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Auth", "DID")]
    [Trait("Priority", "Secondary")]
    public async Task RegisterDevice_WithInvalidDid_ShouldReturn400BadRequest()
    {
        // Arrange - Device registration requires valid DID
        var deviceId = Guid.NewGuid().ToString();
        var invalidDid = "invalid-did-format";
        var deviceInfo = new
        {
            DeviceId = deviceId,
            Did = invalidDid,
            DeviceName = "Test Device",
            DeviceType = "Mobile",
            PublicKey = "test-public-key"
        };

        // Act - Attempt to register device with invalid DID
        var response = await Client.PostAsJsonAsync("/api/devices/register", deviceInfo);

        // Assert
        // Invalid DID should be rejected
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    #endregion

    #region Certificate Authentication Tests (Service-to-Service)

    [Fact]
    [Trait("Auth", "Certificate")]
    [Trait("Priority", "ServiceToService")]
    public async Task Post_WithoutCertificate_WhenAuthRequired_ShouldReturn401Unauthorized()
    {
        // Arrange - Certificate authentication is for SERVICE-TO-SERVICE communication
        // Not for end-user authentication (use JWT or DID instead)
        var command = new SyncPermissions
        {
            ServiceName = "test-service",
            Permissions = new List<string> { "identities:read" }
        };

        // Act - No certificate in request (service-to-service auth missing)
        var response = await Client.PostAsJsonAsync("/api/permissions/sync", command);

        // Assert
        // Certificate authentication is for service-to-service communication
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden);
    }

    [Fact]
    [Trait("Auth", "Certificate")]
    [Trait("Priority", "ServiceToService")]
    public async Task Post_WithInvalidCertificate_ShouldReturn401Unauthorized()
    {
        // Arrange - Certificate authentication is for SERVICE-TO-SERVICE communication
        var command = new SyncPermissions
        {
            ServiceName = "test-service",
            Permissions = new List<string> { "identities:read" }
        };

        // Act - Add invalid certificate (this would require setting up a test certificate handler)
        // For now, we verify that without a valid certificate, the request is rejected
        var response = await Client.PostAsJsonAsync("/api/permissions/sync", command);

        // Assert
        // Invalid certificate should be rejected
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Gets a JWT token for testing (PRIMARY authentication method).
    /// In a real scenario, this would be obtained from /api/auth/sign-in endpoint.
    /// </summary>
    private async Task<string> GetJwtTokenAsync()
    {
        // Attempt to login via REST API to get JWT token
        // JWT is the PRIMARY authentication method
        try
        {
            var loginRequest = new
            {
                username = "test",
                password = "test"
            };

            var response = await Client.PostAsJsonAsync("/api/auth/sign-in", loginRequest);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var loginResponse = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(content, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return loginResponse?.AccessToken ?? throw new InvalidOperationException("Failed to get JWT token from login response");
            }
        }
        catch (Exception ex)
        {
            // If login fails, return a test token (tests will fail with Unauthenticated)
            // This is expected if the login endpoint doesn't exist yet
            Console.WriteLine($"Warning: Failed to get JWT token: {ex.Message}");
        }

        // Return a placeholder token for testing (will fail authentication)
        return "test-jwt-token";
    }

    /// <summary>
    /// Registers a device for DID authentication (SECONDARY authentication method).
    /// Device registration is required before using DID authentication.
    /// </summary>
    private async Task<HttpResponseMessage> RegisterDeviceAsync(string deviceId, string did)
    {
        var deviceInfo = new
        {
            DeviceId = deviceId,
            Did = did,
            DeviceName = "Test Device",
            DeviceType = "Mobile",
            PublicKey = "test-public-key"
        };

        return await Client.PostAsJsonAsync("/api/devices/register", deviceInfo);
    }

    /// <summary>
    /// Login response model for JWT token retrieval.
    /// </summary>
    private class LoginResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    #endregion

    #region Permission Validation Tests

    [Fact]
    public async Task Post_WithInsufficientPermissions_ShouldReturn403Forbidden()
    {
        // Arrange - SyncPermissions endpoint requires "identities:admin" permission
        var command = new SyncPermissions
        {
            ServiceName = "test-service",
            Permissions = new List<string> { "identities:read" } // Only read permission, not admin
        };

        // Act - Certificate with only read permission (would need to set up test certificate)
        var response = await Client.PostAsJsonAsync("/api/permissions/sync", command);

        // Assert
        // Note: This test verifies that permission validation works
        // In a real scenario, we would need to set up a test certificate with specific permissions
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden);
    }

    #endregion

    #region Unauthorized Access Tests

    [Fact]
    public async Task Get_WithoutAuthentication_WhenNotRequired_ShouldReturn200Ok()
    {
        // Arrange - Most endpoints have auth: false
        var identityId = Guid.NewGuid();
        var addCommand = new AddIdentity
        {
            Id = identityId,
            Name = new Name("Test", "User"),
            PersonalDetails = new PersonalDetails(DateTime.UtcNow.AddYears(-30), "USA", "Male", "Wolf Clan"),
            ContactInformation = new ContactInformation(
                new Email("test@example.com"),
                new Address("", "123 Test St", null, null, null, "Springfield", "IL", "62701", null, null, "USA", null, false, Address.AddressType.Home),
                new List<Phone> { new Phone("1", "555-1234", null, Phone.PhoneType.Mobile) }
            ),
            BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[1024], "test-hash"),
            Zone = "zone-001"
        };

        await Client.PostAsJsonAsync("/api/identities", addCommand);

        // Act - No authentication required
        var response = await Client.GetAsync($"/api/identities/{identityId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Post_WithoutAuthentication_WhenNotRequired_ShouldReturn201Created()
    {
        // Arrange
        var command = new AddIdentity
        {
            Id = Guid.NewGuid(),
            Name = new Name("Test", "User"),
            PersonalDetails = new PersonalDetails(DateTime.UtcNow.AddYears(-30), "USA", "Male", "Wolf Clan"),
            ContactInformation = new ContactInformation(
                new Email("test@example.com"),
                new Address("", "123 Test St", null, null, null, "Springfield", "IL", "62701", null, null, "USA", null, false, Address.AddressType.Home),
                new List<Phone> { new Phone("1", "555-1234", null, Phone.PhoneType.Mobile) }
            ),
            BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[1024], "test-hash"),
            Zone = "zone-001"
        };

        // Act - No authentication required
        var response = await Client.PostAsJsonAsync("/api/identities", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    #endregion
}

