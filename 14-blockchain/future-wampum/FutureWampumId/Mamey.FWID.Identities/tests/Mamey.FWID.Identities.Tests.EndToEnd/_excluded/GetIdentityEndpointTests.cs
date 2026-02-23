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
/// End-to-end tests for GET /api/identities/{id} endpoint.
/// </summary>
[Collection("EndToEnd")]
public class GetIdentityEndpointTests : BaseApiEndpointTests
{
    public GetIdentityEndpointTests(
        PostgreSQLFixture postgresFixture,
        RedisFixture redisFixture,
        MongoDBFixture mongoFixture,
        MinIOFixture minioFixture)
        : base(postgresFixture, redisFixture, mongoFixture, minioFixture)
    {
    }

    [Fact]
    public async Task Get_WithExistingIdentity_ShouldReturn200Ok()
    {
        // Arrange - Create an identity first
        var identityId = Guid.NewGuid();
        var addCommand = new AddIdentity
        {
            Id = identityId,
            Name = new Name("Jane", "Smith"),
            PersonalDetails = new PersonalDetails(DateTime.UtcNow.AddYears(-25), "USA", "Female", "Wolf Clan"),
            ContactInformation = new ContactInformation(
                new Email("jane.smith@example.com"),
                new Address("", "456 Oak Ave", null, null, null, "Chicago", "IL", "60601", null, null, "USA", null, false, Address.AddressType.Home),
                new List<Phone> { new Phone("1", "555-5678", null, Phone.PhoneType.Mobile) }
            ),
            BiometricData = new BiometricData(BiometricType.Facial, new byte[2048], "test-hash"),
            Zone = "zone-002"
        };

        await Client.PostAsJsonAsync("/api/identities", addCommand);

        // Act
        var response = await Client.GetAsync($"/api/identities/{identityId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var identity = await response.Content.ReadFromJsonAsync<IdentityDto>();
        identity.ShouldNotBeNull();
        identity.Id.Value.ShouldBe(identityId);
        identity.Name.FirstName.ShouldBe("Jane");
    }

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
}

