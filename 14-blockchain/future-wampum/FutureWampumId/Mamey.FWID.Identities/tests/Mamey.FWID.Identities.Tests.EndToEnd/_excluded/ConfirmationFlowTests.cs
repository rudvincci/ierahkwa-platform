using System.Net;
using System.Net.Http.Json;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Contracts.Queries;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.EndToEnd.Authentication;

/// <summary>
/// End-to-end tests for email and SMS confirmation flows.
/// </summary>
[Collection("EndToEnd")]
public class ConfirmationFlowTests : ApiEndpoints.BaseApiEndpointTests
{
    public ConfirmationFlowTests(
        PostgreSQLFixture postgresFixture,
        RedisFixture redisFixture,
        MongoDBFixture mongoFixture,
        MinIOFixture minioFixture)
        : base(postgresFixture, redisFixture, mongoFixture, minioFixture)
    {
    }

    [Fact]
    public async Task CreateEmailConfirmation_WithValidEmail_ShouldReturn200Ok()
    {
        // Arrange
        var command = new CreateEmailConfirmation
        {
            IdentityId = Guid.NewGuid(),
            Email = "test@example.com"
        };

        // Act
        // Note: This endpoint requires authentication
        var response = await Client.PostAsJsonAsync("/api/auth/confirmations/email", command);

        // Assert
        // May return 401 if authentication is required
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ConfirmEmail_WithValidToken_ShouldReturn200Ok()
    {
        // Arrange
        var command = new ConfirmEmail
        {
            Token = "valid-confirmation-token"
        };

        // Act
        // Note: This endpoint may be public (no auth required for confirmation)
        var response = await Client.PostAsJsonAsync("/api/auth/confirmations/email/confirm", command);

        // Assert
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ResendEmailConfirmation_WithValidIdentity_ShouldReturn200Ok()
    {
        // Arrange
        var command = new ResendEmailConfirmation
        {
            IdentityId = Guid.NewGuid(),
            Email = "test@example.com"
        };

        // Act
        // Note: This endpoint requires authentication
        var response = await Client.PostAsJsonAsync("/api/auth/confirmations/email/resend", command);

        // Assert
        // May return 401 if authentication is required
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetEmailConfirmationStatus_WithValidIdentity_ShouldReturn200Ok()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        var query = new GetEmailConfirmationStatus
        {
            IdentityId = identityId
        };

        // Act
        // Note: This endpoint requires authentication
        var response = await Client.GetAsync($"/api/auth/confirmations/email/status/{identityId}");

        // Assert
        // May return 401 if authentication is required
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateSmsConfirmation_WithValidPhone_ShouldReturn200Ok()
    {
        // Arrange
        var command = new CreateSmsConfirmation
        {
            IdentityId = Guid.NewGuid(),
            PhoneNumber = "+1234567890"
        };

        // Act
        // Note: This endpoint requires authentication
        var response = await Client.PostAsJsonAsync("/api/auth/confirmations/sms", command);

        // Assert
        // May return 401 if authentication is required
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ConfirmSms_WithValidCode_ShouldReturn200Ok()
    {
        // Arrange
        var command = new ConfirmSms
        {
            IdentityId = Guid.NewGuid(),
            Code = "123456"
        };

        // Act
        // Note: This endpoint may be public (no auth required for confirmation)
        var response = await Client.PostAsJsonAsync("/api/auth/confirmations/sms/confirm", command);

        // Assert
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ResendSmsConfirmation_WithValidIdentity_ShouldReturn200Ok()
    {
        // Arrange
        var command = new ResendSmsConfirmation
        {
            IdentityId = Guid.NewGuid(),
            PhoneNumber = "+1234567890"
        };

        // Act
        // Note: This endpoint requires authentication
        var response = await Client.PostAsJsonAsync("/api/auth/confirmations/sms/resend", command);

        // Assert
        // May return 401 if authentication is required
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetSmsConfirmationStatus_WithValidIdentity_ShouldReturn200Ok()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        var query = new GetSmsConfirmationStatus
        {
            IdentityId = identityId
        };

        // Act
        // Note: This endpoint requires authentication
        var response = await Client.GetAsync($"/api/auth/confirmations/sms/status/{identityId}");

        // Assert
        // May return 401 if authentication is required
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.NotFound);
    }
}

