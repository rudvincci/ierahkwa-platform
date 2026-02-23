using Mamey.FWID.Identities.Application.Exceptions;
using System;
using Mamey.FWID.Identities.Application.Queries.Handlers;
using Mamey.FWID.Identities.Contracts.DTOs;
using Mamey.FWID.Identities.Contracts.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Application.Queries.Handlers;

public class GetIdentityHandlerTests
{
    private readonly IIdentityRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly GetIdentityHandler _handler;

    public GetIdentityHandlerTests()
    {
        _repository = Substitute.For<IIdentityRepository>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _handler = new GetIdentityHandler(_repository, _cache);
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityExists_ShouldReturnIdentityDto()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var query = new GetIdentity(identityId);

        var identity = new Identity(
            identityId,
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

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(identityId);
        result.Name.ShouldBe(identity.Name);
        result.Status.ShouldBe(identity.Status);
        result.Zone.ShouldBe(identity.Zone);
        await _repository.Received(1).GetAsync(identityId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var query = new GetIdentity(identityId);

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns((Identity?)null);

        // Act & Assert
        var exception = await Should.ThrowAsync<IdentityNotFoundException>(
            () => _handler.HandleAsync(query));

        exception.IdentityId.ShouldBe(identityId);
    }

    [Fact]
    public async Task HandleAsync_WhenCached_ShouldReturnCachedDto()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var query = new GetIdentity(identityId);
        var cacheKey = $"identity:{identityId.Value}";

        var cachedDto = new IdentityDto
        {
            Id = identityId,
            Name = new Name("John", "Doe", "M."),
            Status = IdentityStatus.Verified,
            Zone = "zone-001",
            CreatedAt = DateTime.UtcNow
        };

        _cache.Set(cacheKey, cachedDto, TimeSpan.FromMinutes(5));

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(identityId);
        result.Name.ShouldBe(cachedDto.Name);
        await _repository.DidNotReceive().GetAsync(Arg.Any<IdentityId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenNotCached_ShouldCacheResult()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var query = new GetIdentity(identityId);
        var cacheKey = $"identity:{identityId.Value}";

        var identity = new Identity(
            identityId,
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

        _repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        _cache.TryGetValue(cacheKey, out IdentityDto? cached).ShouldBeTrue();
        cached.ShouldNotBeNull();
        cached!.Id.ShouldBe(identityId);
    }
}

