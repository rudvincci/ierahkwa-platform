using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
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
/// End-to-end tests for POST /api/identities endpoint.
/// </summary>
[Collection("EndToEnd")]
public class AddIdentityEndpointTests : BaseApiEndpointTests
{
    public AddIdentityEndpointTests(
        PostgreSQLFixture postgresFixture,
        RedisFixture redisFixture,
        MongoDBFixture mongoFixture,
        MinIOFixture minioFixture)
        : base(postgresFixture, redisFixture, mongoFixture, minioFixture)
    {
    }

    [Fact]
    public async Task Post_WithValidCommand_ShouldReturn201Created()
    {
        // Arrange
        var command = new AddIdentity
        {
            Id = Guid.NewGuid(),
            Name = new Name("John", "Doe"),
            PersonalDetails = new PersonalDetails(DateTime.UtcNow.AddYears(-30), "USA", "Male", "Wolf Clan"),
            ContactInformation = new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "Springfield", "IL", "62701", null, null, "USA", null, false, Address.AddressType.Home),
                new List<Phone> { new Phone("1", "555-1234", null, Phone.PhoneType.Mobile) }
            ),
            BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[1024], "test-hash"),
            Zone = "zone-001"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/identities", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("Identity registered successfully");
    }

    [Fact]
    public async Task Post_WithInvalidCommand_ShouldReturn400BadRequest()
    {
        // Arrange
        var command = new AddIdentity
        {
            Id = Guid.NewGuid(),
            Name = null!, // Invalid - required field
            PersonalDetails = new PersonalDetails(DateTime.UtcNow.AddYears(-30), "USA", "Male", "Wolf Clan"),
            ContactInformation = new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "Springfield", "IL", "62701", null, null, "USA", null, false, Address.AddressType.Home),
                new List<Phone> { new Phone("1", "555-1234", null, Phone.PhoneType.Mobile) }
            ),
            BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[1024], "test-hash")
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/identities", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}

