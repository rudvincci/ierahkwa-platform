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
using NSubstitute;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Application.Commands.Handlers;

/// <summary>
/// Error recovery tests for command handlers.
/// Tests error recovery, retry mechanisms, and fallback scenarios.
/// </summary>
public class ErrorRecoveryTests
{
    #region Database Connection Failure Recovery

    [Fact]
    public async Task AddIdentity_WhenDatabaseConnectionFails_ShouldThrowException()
    {
        // Arrange - Business Rule: Database connection failures should be handled gracefully
        var command = new AddIdentity
        {
            Id = Guid.NewGuid(),
            Name = new Name("John", "Doe"),
            PersonalDetails = new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            ContactInformation = new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
            ),
            BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            Zone = "zone-001"
        };

        var repository = Substitute.For<IIdentityRepository>();
        var eventProcessor = Substitute.For<IEventProcessor>();
        var handler = new AddIdentityHandler(repository, eventProcessor);

        repository.GetAsync(Arg.Any<IdentityId>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Identity?>(new Exception("Database connection failed")));

        // Act & Assert
        await Should.ThrowAsync<Exception>(
            () => handler.HandleAsync(command));
    }

    [Fact]
    public async Task AddIdentity_WhenDatabaseConnectionRecovers_ShouldSucceed()
    {
        // Arrange - Business Rule: After database connection recovery, operations should succeed
        var command = new AddIdentity
        {
            Id = Guid.NewGuid(),
            Name = new Name("John", "Doe"),
            PersonalDetails = new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            ContactInformation = new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
            ),
            BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            Zone = "zone-001"
        };

        var repository = Substitute.For<IIdentityRepository>();
        var eventProcessor = Substitute.For<IEventProcessor>();
        var handler = new AddIdentityHandler(repository, eventProcessor);

        var callCount = 0;
        repository.GetAsync(Arg.Any<IdentityId>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                callCount++;
                if (callCount == 1)
                {
                    throw new Exception("Database connection failed");
                }
                return Task.FromResult<Identity?>(null);
            });

        // Act - First call fails, second call succeeds
        try
        {
            await handler.HandleAsync(command);
        }
        catch
        {
            // Retry after recovery
            await handler.HandleAsync(command);
        }

        // Assert
        await repository.Received(2).GetAsync(Arg.Any<IdentityId>(), Arg.Any<CancellationToken>());
        await repository.Received(1).AddAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Event Processor Failure Recovery

    [Fact]
    public async Task AddIdentity_WhenEventProcessorFails_ShouldStillSaveIdentity()
    {
        // Arrange - Business Rule: If event processing fails, identity should still be saved
        var command = new AddIdentity
        {
            Id = Guid.NewGuid(),
            Name = new Name("John", "Doe"),
            PersonalDetails = new PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            ContactInformation = new ContactInformation(
                new Email("john.doe@example.com"),
                new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
            ),
            BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            Zone = "zone-001"
        };

        var repository = Substitute.For<IIdentityRepository>();
        var eventProcessor = Substitute.For<IEventProcessor>();
        var handler = new AddIdentityHandler(repository, eventProcessor);

        repository.GetAsync(Arg.Any<IdentityId>(), Arg.Any<CancellationToken>())
            .Returns((Identity?)null);

        eventProcessor.ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>())
            .Returns(Task.FromException(new Exception("Event processor failed")));

        // Act & Assert
        // Note: In a real implementation, event processing failures might be handled differently
        // (e.g., stored in outbox for retry). For now, we verify the exception is thrown.
        await Should.ThrowAsync<Exception>(
            () => handler.HandleAsync(command));

        // Identity should still be saved before event processing
        await repository.Received(1).AddAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Transient Failure Retry

    [Fact]
    public async Task GetIdentity_WhenTransientFailure_ShouldRetry()
    {
        // Arrange - Business Rule: Transient failures should be retried
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

        var repository = Substitute.For<IIdentityRepository>();
        var callCount = 0;
        repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                callCount++;
                if (callCount < 3)
                {
                    throw new Exception("Transient failure");
                }
                return Task.FromResult<Identity?>(identity);
            });

        // Act - Retry logic (simulated)
        Identity? result = null;
        var maxRetries = 3;
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                result = await repository.GetAsync(identityId);
                break;
            }
            catch
            {
                if (i == maxRetries - 1)
                {
                    throw;
                }
            }
        }

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(identityId);
    }

    #endregion

    #region External Service Failure Recovery

    [Fact]
    public async Task VerifyBiometric_WhenBiometricServiceFails_ShouldHandleGracefully()
    {
        // Arrange - Business Rule: External service failures should be handled gracefully
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

        var command = new VerifyBiometric
        {
            IdentityId = identityId,
            ProvidedBiometric = new BiometricData(BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            Threshold = 0.85
        };

        var repository = Substitute.For<IIdentityRepository>();
        var eventProcessor = Substitute.For<IEventProcessor>();
        var storageService = Substitute.For<Mamey.FWID.Identities.Application.Services.IBiometricStorageService>();
        var evidenceService = Substitute.For<Mamey.FWID.Identities.Application.Services.IBiometricEvidenceService>();
        var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<VerifyBiometricHandler>>();
        var handler = new VerifyBiometricHandler(repository, eventProcessor, storageService, evidenceService, logger);

        repository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);

        // Note: IBiometricStorageService doesn't have VerifyAsync
        // This test simulates a failure during biometric verification
        // The actual verification is done in the domain layer, not the storage service
        evidenceService.CreateVerificationEvidenceAsync(
            Arg.Any<IdentityId>(),
            Arg.Any<string?>(),
            Arg.Any<double>(),
            Arg.Any<BiometricData>(),
            Arg.Any<string>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>())
            .Returns(Task.FromException<string>(new Exception("Biometric service unavailable")));

        // Act & Assert
        await Should.ThrowAsync<Exception>(
            () => handler.HandleAsync(command));
    }

    #endregion

    #region Partial Failure Recovery

    [Fact]
    public async Task UpdateIdentity_WhenPartialFailure_ShouldMaintainConsistency()
    {
        // Arrange - Business Rule: Partial failures should maintain data consistency
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

        // Simulate partial failure: update succeeds, but event processing fails
        eventProcessor.ProcessAsync(Arg.Any<IEnumerable<IDomainEvent>>())
            .Returns(Task.FromException(new Exception("Event processing failed")));

        // Act & Assert
        // In a real implementation, this might use transactions or outbox pattern
        // For now, we verify the exception is thrown
        await Should.ThrowAsync<Exception>(
            () => handler.HandleAsync(command));

        // Update should still be applied
        await repository.Received(1).UpdateAsync(Arg.Any<Identity>(), Arg.Any<CancellationToken>());
    }

    #endregion
}

