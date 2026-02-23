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
/// End-to-end tests for PUT /api/identities/{id}/contact endpoint.
/// </summary>
[Collection("EndToEnd")]
public class UpdateContactInformationEndpointTests : BaseApiEndpointTests
{
    public UpdateContactInformationEndpointTests(
        PostgreSQLFixture postgresFixture,
        RedisFixture redisFixture,
        MongoDBFixture mongoFixture,
        MinIOFixture minioFixture)
        : base(postgresFixture, redisFixture, mongoFixture, minioFixture)
    {
    }

    [Fact]
    public async Task Put_WithValidCommand_ShouldReturn200Ok()
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
            ContactInformation = new ContactInformation(
                new Email("updated@example.com"),
                new Address("", "999 Updated Ave", null, null, null, "Chicago", "IL", "60601", null, null, "USA", null, false, Address.AddressType.Business),
                new List<Phone> { new Phone("1", "555-9999", null, Phone.PhoneType.Main) }
            )
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/identities/{identityId}/contact", updateCommand);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("Contact information updated successfully");

        // Verify contact information is updated
        var getResponse = await Client.GetAsync($"/api/identities/{identityId}");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var identity = await getResponse.Content.ReadFromJsonAsync<IdentityDto>();
        identity.ShouldNotBeNull();
        // TODO: IdentityDto doesn't expose ContactInformation - need to check actual DTO structure
    }

    [Fact]
    public async Task Put_WithNonExistentIdentity_ShouldReturn404NotFound()
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
}

