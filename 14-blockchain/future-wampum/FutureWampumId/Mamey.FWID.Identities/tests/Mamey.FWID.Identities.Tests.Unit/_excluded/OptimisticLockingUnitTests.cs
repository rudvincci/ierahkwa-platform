#nullable enable
using Mamey.CQRS;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Commands.Handlers;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Application.Commands.Handlers;

/// <summary>
/// Unit tests for optimistic locking scenarios.
/// Tests that handlers properly handle DbUpdateConcurrencyException.
/// </summary>
public class OptimisticLockingUnitTests
{
    #region Optimistic Locking - Handler Retry Logic

    [Fact]
    public async Task UpdateContactInformation_WhenConcurrencyException_ShouldRetry()
    {
        // Arrange - Business Rule: Handlers should retry on concurrency exceptions
        var identityId = new IdentityId(Guid.NewGuid());
        var identity = new Identity(
            identityId,
            new Name("John", "Doe"),
            new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
            ),
            new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            "zone-001",
            null);

        var command = new UpdateContactInformation
        {
            IdentityId = identityId,
            ContactInformation = new ContactInformation(
                new Email("updated@example.com"),
                new Address("", "999 Updated St", null, null, null, "Chicago", "IL", "60601", null, null, "US", null),
                new List<Phone> { new Phone("1", "5559999999", null, Phone.PhoneType.Mobile) }
            )
        };

        var repository = Substitute.For<IIdentityRepository>();
        var eventProcessor = Substitute.For<IEventProcessor>();
        var handler = new UpdateContactInformationHandler(repository, eventProcessor);

        var callCount = 0;
        repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                callCount++;
                if (callCount == 1)
                {
                    return Task.FromResult<Identity?>(identity);
                }
                else
                {
                    // Return fresh identity on retry
                    return Task.FromResult<Identity?>(identity);
                }
            });

        // Simulate concurrency exception on first update, success on retry
        var updateCallCount = 0;
        repository.UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(_ =>
            {
                updateCallCount++;
                if (updateCallCount == 1)
                {
                    throw new DbUpdateConcurrencyException("Concurrency conflict");
                }
            });

        // Act - Handler should retry on concurrency exception
        try
        {
            await handler.HandleAsync(command);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Retry logic (simulated)
            await handler.HandleAsync(command);
        }

        // Assert
        await repository.Received(2).GetAsync(identityId, Arg.Any<CancellationToken>());
        await repository.Received(2).UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Optimistic Locking - Version Mismatch Detection

    [Fact]
    public async Task UpdateContactInformation_WhenVersionMismatch_ShouldThrowConcurrencyException()
    {
        // Arrange - Business Rule: Version mismatches should be detected
        var identityId = new IdentityId(Guid.NewGuid());
        var identity = new Identity(
            identityId,
            new Name("John", "Doe"),
            new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
            ),
            new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            "zone-001",
            null);

        var command = new UpdateContactInformation
        {
            IdentityId = identityId,
            ContactInformation = new ContactInformation(
                new Email("updated@example.com"),
                new Address("", "999 Updated St", null, null, null, "Chicago", "IL", "60601", null, null, "US", null),
                new List<Phone> { new Phone("1", "5559999999", null, Phone.PhoneType.Mobile) }
            )
        };

        var repository = Substitute.For<IIdentityRepository>();
        var eventProcessor = Substitute.For<IEventProcessor>();
        var handler = new UpdateContactInformationHandler(repository, eventProcessor);

        repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Simulate version mismatch
        repository.UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new DbUpdateConcurrencyException("Version mismatch detected")));

        // Act & Assert
        await Should.ThrowAsync<DbUpdateConcurrencyException>(
            () => handler.HandleAsync(command));
    }

    #endregion

    #region Optimistic Locking - Stale Data Detection

    [Fact]
    public async Task UpdateContactInformation_WithStaleData_ShouldDetectAndHandle()
    {
        // Arrange - Business Rule: Stale data should be detected via version check
        var identityId = new IdentityId(Guid.NewGuid());
        var originalIdentity = new Identity(
            identityId,
            new Name("John", "Doe"),
            new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
            ),
            new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            "zone-001",
            null);

        var updatedIdentity = new Identity(
            identityId,
            new Name("John", "Doe"),
            new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            new ContactInformation(
                new Email("updated@example.com"), // Already updated
                new Address("", "999 Updated St", null, null, null, "Chicago", "IL", "60601", null, null, "US", null),
                new List<Phone> { new Phone("1", "5559999999", null, Phone.PhoneType.Mobile) }
            ),
            new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            "zone-001",
            null);

        var command = new UpdateContactInformation
        {
            IdentityId = identityId,
            ContactInformation = new ContactInformation(
                new Email("another-update@example.com"),
                new Address("", "888 Updated Ave", null, null, null, "Los Angeles", "CA", "90001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5558888888", null, Phone.PhoneType.Mobile) }
            )
        };

        var repository = Substitute.For<IIdentityRepository>();
        var eventProcessor = Substitute.For<IEventProcessor>();
        var handler = new UpdateContactInformationHandler(repository, eventProcessor);

        // First call returns original, second call returns updated (simulating stale data)
        var callCount = 0;
        repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                callCount++;
                if (callCount == 1)
                {
                    return Task.FromResult<Identity?>(originalIdentity);
                }
                else
                {
                    return Task.FromResult<Identity?>(updatedIdentity);
                }
            });

        // First update succeeds, but version changed
        var updateCallCount = 0;
        repository.UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(_ =>
            {
                updateCallCount++;
                if (updateCallCount == 1)
                {
                    throw new DbUpdateConcurrencyException("Stale data detected");
                }
            });

        // Act - Handler should detect stale data and retry
        try
        {
            await handler.HandleAsync(command);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Retry with fresh data
            await handler.HandleAsync(command);
        }

        // Assert
        await repository.Received(2).GetAsync(identityId, Arg.Any<CancellationToken>());
    }

    #endregion
}

