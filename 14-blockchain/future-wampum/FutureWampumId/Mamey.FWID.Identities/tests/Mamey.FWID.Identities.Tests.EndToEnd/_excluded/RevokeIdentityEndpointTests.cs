using System.Net;
using System.Net.Http.Json;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Contracts.DTOs;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.EndToEnd.ApiEndpoints;

/// <summary>
/// End-to-end tests for POST /api/identities/{id}/revoke endpoint.
/// </summary>
[Collection("EndToEnd")]
public class RevokeIdentityEndpointTests : BaseApiEndpointTests
{
    public RevokeIdentityEndpointTests(
        PostgreSQLFixture postgresFixture,
        RedisFixture redisFixture,
        MongoDBFixture mongoFixture,
        MinIOFixture minioFixture)
        : base(postgresFixture, redisFixture, mongoFixture, minioFixture)
    {
    }

    [Fact]
    public async Task Post_WithValidCommand_ShouldReturn200Ok()
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

        var revokeCommand = new RevokeIdentity
        {
            Reason = "Test revocation",
            RevokedBy = Guid.NewGuid()
        };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/identities/{identityId}/revoke", revokeCommand);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("Identity revoked successfully");

        // Verify identity is revoked
        var getResponse = await Client.GetAsync($"/api/identities/{identityId}");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var identity = await getResponse.Content.ReadFromJsonAsync<IdentityDto>();
        identity.ShouldNotBeNull();
        identity.Status.ShouldBe(IdentityStatus.Revoked);
    }

    [Fact]
    public async Task Post_WithNonExistentIdentity_ShouldReturn404NotFound()
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
}

