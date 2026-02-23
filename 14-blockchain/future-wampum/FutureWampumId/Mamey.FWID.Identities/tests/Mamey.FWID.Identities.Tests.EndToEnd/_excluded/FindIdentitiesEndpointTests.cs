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
/// End-to-end tests for GET /api/identities endpoint.
/// </summary>
[Collection("EndToEnd")]
public class FindIdentitiesEndpointTests : BaseApiEndpointTests
{
    public FindIdentitiesEndpointTests(
        PostgreSQLFixture postgresFixture,
        RedisFixture redisFixture,
        MongoDBFixture mongoFixture,
        MinIOFixture minioFixture)
        : base(postgresFixture, redisFixture, mongoFixture, minioFixture)
    {
    }

    [Fact]
    public async Task Get_WithoutFilters_ShouldReturnAllIdentities()
    {
        // Arrange - Create multiple identities
        for (int i = 0; i < 3; i++)
        {
            var command = new AddIdentity
            {
                Id = Guid.NewGuid(),
                Name = new Name($"User{i}", "Test"),
                PersonalDetails = new PersonalDetails(DateTime.UtcNow.AddYears(-30 + i), "USA", "Male", "Wolf Clan"),
                ContactInformation = new ContactInformation(
                    new Email($"user{i}@example.com"),
                    new Address("", $"{100 + i} Main St", null, null, null, "Springfield", "IL", "62701", null, null, "USA", null, false, Address.AddressType.Home),
                    new List<Phone> { new Phone("1", $"555-{1000 + i}", null, Phone.PhoneType.Mobile) }
                ),
                BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[1024], $"hash-{i}"),
                Zone = $"zone-{i:000}"
            };

            await Client.PostAsJsonAsync("/api/identities", command);
        }

        // Act
        var response = await Client.GetAsync("/api/identities");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var identities = await response.Content.ReadFromJsonAsync<List<IdentityDto>>();
        identities.ShouldNotBeNull();
        identities.Count.ShouldBeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task Get_WithZoneFilter_ShouldReturnFilteredIdentities()
    {
        // Arrange - Create identities in different zones
        var zone1Id = Guid.NewGuid();
        var zone2Id = Guid.NewGuid();

        await Client.PostAsJsonAsync("/api/identities", new AddIdentity
        {
            Id = zone1Id,
            Name = new Name("Zone1", "User"),
            PersonalDetails = new PersonalDetails(DateTime.UtcNow.AddYears(-30), "USA", "Male", "Wolf Clan"),
            ContactInformation = new ContactInformation(
                new Email("zone1@example.com"),
                new Address("", "111 Zone St", null, null, null, "Springfield", "IL", "62701", null, null, "USA", null, false, Address.AddressType.Home),
                new List<Phone> { new Phone("1", "555-1111", null, Phone.PhoneType.Mobile) }
            ),
            BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[1024], "hash-1"),
            Zone = "zone-001"
        });

        await Client.PostAsJsonAsync("/api/identities", new AddIdentity
        {
            Id = zone2Id,
            Name = new Name("Zone2", "User"),
            PersonalDetails = new PersonalDetails(DateTime.UtcNow.AddYears(-30), "USA", "Male", "Wolf Clan"),
            ContactInformation = new ContactInformation(
                new Email("zone2@example.com"),
                new Address("", "222 Zone St", null, null, null, "Springfield", "IL", "62701", null, null, "USA", null, false, Address.AddressType.Home),
                new List<Phone> { new Phone("1", "555-2222", null, Phone.PhoneType.Mobile) }
            ),
            BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[1024], "hash-2"),
            Zone = "zone-002"
        });

        // Act
        var response = await Client.GetAsync("/api/identities?Zone=zone-001");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var identities = await response.Content.ReadFromJsonAsync<List<IdentityDto>>();
        identities.ShouldNotBeNull();
        identities.All(i => i.Zone == "zone-001").ShouldBeTrue();
    }
}

