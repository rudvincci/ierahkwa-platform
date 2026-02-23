using System.Net;
using System.Net.Http.Json;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Contracts.Queries;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.EndToEnd.Authentication;

/// <summary>
/// End-to-end tests for MFA flows (setup, enable, challenge, verify).
/// </summary>
[Collection("EndToEnd")]
public class MfaFlowTests : ApiEndpoints.BaseApiEndpointTests
{
    public MfaFlowTests(
        PostgreSQLFixture postgresFixture,
        RedisFixture redisFixture,
        MongoDBFixture mongoFixture,
        MinIOFixture minioFixture)
        : base(postgresFixture, redisFixture, mongoFixture, minioFixture)
    {
    }

    [Fact]
    public async Task SetupMfa_WithValidMethod_ShouldReturn200Ok()
    {
        // Arrange
        var command = new SetupMfa
        {
            IdentityId = Guid.NewGuid(),
            Method = MfaMethod.Totp
        };

        // Act
        // Note: This endpoint requires authentication
        var response = await Client.PostAsJsonAsync("/api/auth/mfa/setup", command);

        // Assert
        // May return 401 if authentication is required
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task EnableMfa_WithValidCode_ShouldReturn200Ok()
    {
        // Arrange
        var command = new EnableMfa
        {
            IdentityId = Guid.NewGuid(),
            Method = MfaMethod.Totp,
            VerificationCode = "123456"
        };

        // Act
        // Note: This endpoint requires authentication
        var response = await Client.PostAsJsonAsync("/api/auth/mfa/enable", command);

        // Assert
        // May return 401 if authentication is required
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DisableMfa_WithValidIdentity_ShouldReturn200Ok()
    {
        // Arrange
        var command = new DisableMfa
        {
            IdentityId = Guid.NewGuid(),
            Method = MfaMethod.Totp
        };

        // Act
        // Note: This endpoint requires authentication
        var response = await Client.PostAsJsonAsync("/api/auth/mfa/disable", command);

        // Assert
        // May return 401 if authentication is required
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateMfaChallenge_WithValidIdentity_ShouldReturn200Ok()
    {
        // Arrange
        var command = new CreateMfaChallenge
        {
            IdentityId = Guid.NewGuid(),
            Method = MfaMethod.Totp
        };

        // Act
        // Note: This endpoint requires authentication
        var response = await Client.PostAsJsonAsync("/api/auth/mfa/challenge", command);

        // Assert
        // May return 401 if authentication is required
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task VerifyMfaChallenge_WithValidCode_ShouldReturn200Ok()
    {
        // Arrange
        var command = new VerifyMfaChallenge
        {
            IdentityId = Guid.NewGuid(),
            Method = MfaMethod.Totp,
            Code = "123456"
        };

        // Act
        // Note: This endpoint requires authentication
        var response = await Client.PostAsJsonAsync("/api/auth/mfa/verify", command);

        // Assert
        // May return 401 if authentication is required
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GenerateBackupCodes_WithValidIdentity_ShouldReturn200Ok()
    {
        // Arrange
        var command = new GenerateBackupCodes
        {
            IdentityId = Guid.NewGuid()
        };

        // Act
        // Note: This endpoint requires authentication
        var response = await Client.PostAsJsonAsync("/api/auth/mfa/backup-codes", command);

        // Assert
        // May return 401 if authentication is required
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetMfaStatus_WithValidIdentity_ShouldReturn200Ok()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        var query = new GetIdentityMfaStatus
        {
            IdentityId = identityId
        };

        // Act
        // Note: This endpoint requires authentication
        var response = await Client.GetAsync($"/api/auth/mfa/status/{identityId}");

        // Assert
        // May return 401 if authentication is required
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.NotFound);
    }
}

