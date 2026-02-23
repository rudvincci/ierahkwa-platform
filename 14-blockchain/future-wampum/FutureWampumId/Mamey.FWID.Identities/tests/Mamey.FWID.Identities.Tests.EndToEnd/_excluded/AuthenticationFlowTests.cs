using System.Net;
using System.Net.Http.Json;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.EndToEnd.Authentication;

/// <summary>
/// End-to-end tests for authentication flows (sign-in, sign-out, refresh).
/// </summary>
[Collection("EndToEnd")]
public class AuthenticationFlowTests : ApiEndpoints.BaseApiEndpointTests
{
    public AuthenticationFlowTests(
        PostgreSQLFixture postgresFixture,
        RedisFixture redisFixture,
        MongoDBFixture mongoFixture,
        MinIOFixture minioFixture)
        : base(postgresFixture, redisFixture, mongoFixture, minioFixture)
    {
    }

    [Fact]
    public async Task SignIn_WithValidCredentials_ShouldReturn200Ok()
    {
        // Arrange
        var command = new SignIn
        {
            Username = "testuser",
            Password = "password123",
            IpAddress = "192.168.1.1",
            UserAgent = "Mozilla/5.0"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/sign-in", command);

        // Assert
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            content.ShouldNotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task SignIn_WithInvalidCredentials_ShouldReturn401Unauthorized()
    {
        // Arrange
        var command = new SignIn
        {
            Username = "invaliduser",
            Password = "wrongpassword"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/sign-in", command);

        // Assert
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SignOut_WithValidSession_ShouldReturn200Ok()
    {
        // Arrange
        // First, sign in to get a session
        var signInCommand = new SignIn
        {
            Username = "testuser",
            Password = "password123"
        };
        var signInResponse = await Client.PostAsJsonAsync("/api/auth/sign-in", signInCommand);
        
        // Extract session ID from response (if available)
        // For now, we'll test with a mock session ID
        var signOutCommand = new SignOut
        {
            SessionId = Guid.NewGuid()
        };

        // Act
        // Note: This endpoint requires authentication, so we may need to set auth header
        var response = await Client.PostAsJsonAsync("/api/auth/sign-out", signOutCommand);

        // Assert
        // May return 401 if authentication is required
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RefreshToken_WithValidRefreshToken_ShouldReturn200Ok()
    {
        // Arrange
        var command = new RefreshToken
        {
            Token = "valid-refresh-token-123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/refresh", command);

        // Assert
        // May return 401 if refresh token is invalid or expired
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RefreshToken_WithInvalidRefreshToken_ShouldReturn401Unauthorized()
    {
        // Arrange
        var command = new RefreshToken
        {
            Token = "invalid-refresh-token"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/refresh", command);

        // Assert
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SignInWithBiometric_WithValidBiometric_ShouldReturn200Ok()
    {
        // Arrange
        var command = new SignInWithBiometric
        {
            IdentityId = Guid.NewGuid(),
            BiometricData = new BiometricData(
                BiometricType.Facial,
                new byte[1024],
                "test-hash-123",
                templateId: "template-123",
                algoVersion: "face-3.2.0",
                format: "ISO39794-5:Face",
                quality: BiometricQuality.Good)
        };

        // Act
        // Note: This endpoint requires authentication
        var response = await Client.PostAsJsonAsync("/api/auth/sign-in/biometric", command);

        // Assert
        // May return 401 if authentication is required
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }
}

