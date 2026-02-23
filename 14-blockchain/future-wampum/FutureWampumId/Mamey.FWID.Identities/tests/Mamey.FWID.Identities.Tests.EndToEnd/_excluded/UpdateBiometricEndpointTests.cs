using System.Net;
using System.Net.Http.Json;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.EndToEnd.ApiEndpoints;

/// <summary>
/// End-to-end tests for PUT /api/identities/{id}/biometric endpoint.
/// </summary>
[Collection("EndToEnd")]
public class UpdateBiometricEndpointTests : BaseApiEndpointTests
{
    public UpdateBiometricEndpointTests(
        PostgreSQLFixture postgresFixture,
        RedisFixture redisFixture,
        MongoDBFixture mongoFixture,
        MinIOFixture minioFixture)
        : base(postgresFixture, redisFixture, mongoFixture, minioFixture)
    {
    }

    [Fact]
    public async Task Put_WithValidBiometric_ShouldReturn200Ok()
    {
        // Arrange - Create an identity first
        var identityId = Guid.NewGuid();
        var originalBiometric = new byte[1024];
        new Random().NextBytes(originalBiometric);

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
            BiometricData = new BiometricData(BiometricType.Fingerprint, originalBiometric, "original-hash"),
            Zone = "zone-001"
        };

        await Client.PostAsJsonAsync("/api/identities", addCommand);

        var newBiometric = new byte[2048];
        new Random().NextBytes(newBiometric);

        var updateCommand = new UpdateBiometric
        {
            NewBiometric = new BiometricData(BiometricType.Facial, newBiometric, "new-hash"),
            VerificationBiometric = new BiometricData(BiometricType.Fingerprint, originalBiometric, "original-hash")
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/identities/{identityId}/biometric", updateCommand);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("Biometric updated successfully");
    }

    [Fact]
    public async Task Put_WithNonExistentIdentity_ShouldReturn404NotFound()
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
}

