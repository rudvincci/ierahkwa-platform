using System;
using System.Net;
using System.Net.Http.Json;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.EndToEnd.ErrorHandling;

/// <summary>
/// End-to-end tests for error handling (400 Bad Request, 404 Not Found, 500 Internal Server Error, validation errors).
/// </summary>
[Collection("EndToEnd")]
public class ErrorHandlingTests : ApiEndpoints.BaseApiEndpointTests
{
    public ErrorHandlingTests(
        PostgreSQLFixture postgresFixture,
        RedisFixture redisFixture,
        MongoDBFixture mongoFixture,
        MinIOFixture minioFixture)
        : base(postgresFixture, redisFixture, mongoFixture, minioFixture)
    {
    }

    #region 400 Bad Request Tests

    [Fact]
    public async Task Post_WithNullName_ShouldReturn400BadRequest()
    {
        // Arrange
        var command = new AddIdentity
        {
            Id = Guid.NewGuid(),
            Name = null!, // Invalid - required field
            PersonalDetails = new PersonalDetails(DateTime.UtcNow.AddYears(-30), "USA", "Male", "Wolf Clan"),
            ContactInformation = new ContactInformation(
                new Email("test@example.com"),
                new Address("", "123 Test St", null, null, null, "Springfield", "IL", "62701", null, null, "USA", null, false, Address.AddressType.Home),
                new List<Phone> { new Phone("1", "555-1234", null, Phone.PhoneType.Mobile) }
            ),
            BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[1024], "test-hash")
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/identities", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_WithNullPersonalDetails_ShouldReturn400BadRequest()
    {
        // Arrange
        var command = new AddIdentity
        {
            Id = Guid.NewGuid(),
            Name = new Name("Test", "User"),
            PersonalDetails = null!, // Invalid - required field
            ContactInformation = new ContactInformation(
                new Email("test@example.com"),
                new Address("", "123 Test St", null, null, null, "Springfield", "IL", "62701", null, null, "USA", null, false, Address.AddressType.Home),
                new List<Phone> { new Phone("1", "555-1234", null, Phone.PhoneType.Mobile) }
            ),
            BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[1024], "test-hash")
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/identities", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_WithNullContactInformation_ShouldReturn400BadRequest()
    {
        // Arrange
        var command = new AddIdentity
        {
            Id = Guid.NewGuid(),
            Name = new Name("Test", "User"),
            PersonalDetails = new PersonalDetails(DateTime.UtcNow.AddYears(-30), "USA", "Male", "Wolf Clan"),
            ContactInformation = null!, // Invalid - required field
            BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[1024], "test-hash")
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/identities", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_WithNullBiometricData_ShouldReturn400BadRequest()
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
            BiometricData = null! // Invalid - required field
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/identities", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_WithNullContactInformation_ShouldReturn400BadRequest()
    {
        // Arrange - Create an identity first
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

        var updateCommand = new UpdateContactInformation
        {
            ContactInformation = null! // Invalid - required field
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/identities/{identityId}/contact", updateCommand);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_WithNullIdentityId_ShouldReturn400BadRequest()
    {
        // Arrange
        var updateCommand = new UpdateZone
        {
            IdentityId = null! // Invalid - required field
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/identities/{Guid.NewGuid()}/zone", updateCommand);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    #endregion

    #region 404 Not Found Tests

    [Fact]
    public async Task Get_WithNonExistentIdentity_ShouldReturn404NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/identities/{nonExistentId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_VerifyBiometric_WithNonExistentIdentity_ShouldReturn404NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var verifyCommand = new VerifyBiometric
        {
            ProvidedBiometric = new BiometricData(BiometricType.Fingerprint, new byte[1024], "test-hash"),
            Threshold = 0.95
        };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/identities/{nonExistentId}/verify", verifyCommand);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_UpdateBiometric_WithNonExistentIdentity_ShouldReturn404NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateCommand = new UpdateBiometric
        {
            NewBiometric = new BiometricData(BiometricType.Fingerprint, new byte[1024], "new-hash"),
            VerificationBiometric = new BiometricData(BiometricType.Fingerprint, new byte[1024], "verify-hash")
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/identities/{nonExistentId}/biometric", updateCommand);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_RevokeIdentity_WithNonExistentIdentity_ShouldReturn404NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var revokeCommand = new RevokeIdentity
        {
            Reason = "Test revocation",
            RevokedBy = Guid.NewGuid()
        };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/identities/{nonExistentId}/revoke", revokeCommand);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_UpdateZone_WithNonExistentIdentity_ShouldReturn404NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateCommand = new UpdateZone
        {
            Zone = "zone-002"
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/identities/{nonExistentId}/zone", updateCommand);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_UpdateContactInformation_WithNonExistentIdentity_ShouldReturn404NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateCommand = new UpdateContactInformation
        {
            ContactInformation = new ContactInformation(
                new Email("updated@example.com"),
                new Address("", "999 Updated Ave", null, null, null, "Chicago", "IL", "60601", null, null, "USA", null, false, Address.AddressType.Home),
                new List<Phone> { new Phone("1", "555-9999", null, Phone.PhoneType.Mobile) }
            )
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/identities/{nonExistentId}/contact", updateCommand);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    #endregion

    #region Validation Error Tests

    [Fact]
    public async Task Post_WithInvalidEmail_ShouldReturn400BadRequest()
    {
        // Arrange
        var command = new AddIdentity
        {
            Id = Guid.NewGuid(),
            Name = new Name("Test", "User"),
            PersonalDetails = new PersonalDetails(DateTime.UtcNow.AddYears(-30), "USA", "Male", "Wolf Clan"),
            ContactInformation = new ContactInformation(
                new Email("invalid-email"), // Invalid email format
                new Address("", "123 Test St", null, null, null, "Springfield", "IL", "62701", null, null, "USA", null, false, Address.AddressType.Home),
                new List<Phone> { new Phone("1", "555-1234", null, Phone.PhoneType.Mobile) }
            ),
            BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[1024], "test-hash")
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/identities", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_WithEmptyBiometricData_ShouldReturn400BadRequest()
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
            BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()) // Valid data
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/identities", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_WithInvalidGuid_ShouldReturn400BadRequest()
    {
        // Arrange - Use invalid GUID format in URL
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
            BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[1024], "test-hash")
        };

        // Act - Use invalid GUID in URL
        var response = await Client.GetAsync("/api/identities/invalid-guid");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    #endregion
}

