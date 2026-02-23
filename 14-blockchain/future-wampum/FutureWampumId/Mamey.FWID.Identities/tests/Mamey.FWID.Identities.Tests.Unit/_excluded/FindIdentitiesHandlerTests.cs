using Mamey.FWID.Identities.Application.Queries.Handlers;
using System;
using Mamey.FWID.Identities.Contracts.DTOs;
using Mamey.FWID.Identities.Contracts.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Application.Queries.Handlers;

public class FindIdentitiesHandlerTests
{
    private readonly IIdentityRepository _repository;
    private readonly FindIdentitiesHandler _handler;

    public FindIdentitiesHandlerTests()
    {
        _repository = Substitute.For<IIdentityRepository>();
        _handler = new FindIdentitiesHandler(_repository);
    }

    [Fact]
    public async Task HandleAsync_WhenNoFilters_ShouldReturnAllIdentities()
    {
        // Arrange
        var query = new FindIdentities(null, null);

        var identities = new List<Identity>
        {
            new Identity(
                new IdentityId(Guid.NewGuid()),
                new Name("John", "Doe", "M."),
                new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
                new ContactInformation(
                    new Email("john.doe@example.com"),
                    new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                    new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
                ),
                new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
                "zone-001",
                null),
            new Identity(
                new IdentityId(Guid.NewGuid()),
                new Name("Jane", "Smith", "L."),
                new PersonalDetails(new DateTime(1985, 5, 15), "Los Angeles, CA", "Female", "Bear Clan"),
                new ContactInformation(
                    new Email("jane.smith@example.com"),
                    new Address("", "456 Oak Ave", null, null, null, "Los Angeles", "CA", "90001", null, null, "US", null),
                    new List<Phone> { new Phone("1", "5559876543", null, Phone.PhoneType.Mobile) }
                ),
                new BiometricData(BiometricType.Facial, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
                "zone-002",
                null)
        };

        _repository.BrowseAsync(Arg.Any<CancellationToken>())
            .Returns(identities);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        await _repository.Received(1).BrowseAsync(Arg.Any<CancellationToken>());
        await _repository.DidNotReceive().FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Identity, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenZoneFilter_ShouldReturnFilteredIdentities()
    {
        // Arrange
        var zone = "zone-001";
        var query = new FindIdentities(zone, null);

        var identities = new List<Identity>
        {
            new Identity(
                new IdentityId(Guid.NewGuid()),
                new Name("John", "Doe", "M."),
                new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
                new ContactInformation(
                    new Email("john.doe@example.com"),
                    new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                    new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
                ),
                new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
                zone,
                null)
        };

        _repository.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Identity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(identities);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result[0].Zone.ShouldBe(zone);
        await _repository.Received(1).FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Identity, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenStatusFilter_ShouldReturnFilteredIdentities()
    {
        // Arrange
        var status = IdentityStatus.Verified;
        var query = new FindIdentities(null, status);

        var identity = new Identity(
            new IdentityId(Guid.NewGuid()),
            new Name("John", "Doe", "M."),
            new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
            ),
            new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            "zone-001",
            null);
        
        // Verify the identity to set status to Verified
        identity.VerifyBiometric(identity.BiometricData);
        
        var identities = new List<Identity> { identity };

        _repository.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Identity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(identities);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result[0].Status.ShouldBe(status);
        await _repository.Received(1).FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Identity, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenZoneAndStatusFilter_ShouldReturnFilteredIdentities()
    {
        // Arrange
        var zone = "zone-001";
        var status = IdentityStatus.Verified;
        var query = new FindIdentities(zone, status);

        var identity = new Identity(
            new IdentityId(Guid.NewGuid()),
            new Name("John", "Doe", "M."),
            new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
            ),
            new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            zone,
            null);
        
        // Verify the identity to set status to Verified
        identity.VerifyBiometric(identity.BiometricData);
        
        var identities = new List<Identity> { identity };

        _repository.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Identity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(identities);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result[0].Zone.ShouldBe(zone);
        result[0].Status.ShouldBe(status);
        await _repository.Received(1).FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Identity, bool>>>(), Arg.Any<CancellationToken>());
    }
}

