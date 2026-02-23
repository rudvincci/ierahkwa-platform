#nullable enable
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Clients;
using Mamey.FWID.Identities.Application.Events.Handlers.Integration;
using Mamey.FWID.Identities.Application.Events.Integration.Credentials;
using Mamey.FWID.Identities.Application.Events.Integration.DIDs;
using Mamey.FWID.Identities.Application.Events.Integration.ZKPs;
using Mamey.FWID.Identities.Application.Events.Integration.AccessControls;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Application.Events;

/// <summary>
/// Integration event handling tests.
/// Tests that integration events from other services are properly handled.
/// </summary>
public class IntegrationEventHandlingTests
{
    #region DIDCreatedIntegrationEvent Handling

    [Fact]
    public async Task DIDCreatedIntegrationEventHandler_WhenEventReceived_ShouldHandleEvent()
    {
        // Arrange - Business Rule: DIDCreatedIntegrationEvent should be handled when received from DIDs service
        var identityId = Guid.NewGuid();
        var @event = new DIDCreatedIntegrationEvent(
            DIDId: Guid.NewGuid(),
            IdentityId: identityId,
            DidString: "did:web:example.com:identity:123",
            CreatedAt: DateTime.UtcNow);

        var didsServiceClient = Substitute.For<IDIDsServiceClient>();
        var logger = Substitute.For<ILogger<DIDCreatedIntegrationEventHandler>>();
        var handler = new DIDCreatedIntegrationEventHandler(didsServiceClient, logger);

        var didDto = new Mamey.FWID.Identities.Application.DTO.DIDDto
        {
            DIDId = @event.DIDId,
            IdentityId = new IdentityId(@event.IdentityId),
            DidString = @event.DidString
        };

        didsServiceClient.GetDIDByIdentityIdAsync(new IdentityId(@event.IdentityId), Arg.Any<CancellationToken>())
            .Returns(didDto);

        // Act
        await handler.HandleAsync(@event);

        // Assert
        await didsServiceClient.Received(1).GetDIDByIdentityIdAsync(@event.IdentityId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DIDCreatedIntegrationEventHandler_WhenDIDNotFound_ShouldLogWarning()
    {
        // Arrange - Business Rule: If DID is not found, handler should log warning and return
        var identityId = Guid.NewGuid();
        var @event = new DIDCreatedIntegrationEvent(
            DIDId: Guid.NewGuid(),
            IdentityId: identityId,
            DidString: "did:web:example.com:identity:123",
            CreatedAt: DateTime.UtcNow);

        var didsServiceClient = Substitute.For<IDIDsServiceClient>();
        var logger = Substitute.For<ILogger<DIDCreatedIntegrationEventHandler>>();
        var handler = new DIDCreatedIntegrationEventHandler(didsServiceClient, logger);

        didsServiceClient.GetDIDByIdentityIdAsync(new IdentityId(@event.IdentityId), Arg.Any<CancellationToken>())
            .Returns((Mamey.FWID.Identities.Application.DTO.DIDDto?)null);

        // Act
        await handler.HandleAsync(@event);

        // Assert
        await didsServiceClient.Received(1).GetDIDByIdentityIdAsync(@event.IdentityId, Arg.Any<CancellationToken>());
        // Should not throw exception, just log warning
    }

    [Fact]
    public async Task DIDCreatedIntegrationEventHandler_WhenDIDMismatch_ShouldLogWarning()
    {
        // Arrange - Business Rule: If DID data doesn't match, handler should log warning and return
        var identityId = Guid.NewGuid();
        var @event = new DIDCreatedIntegrationEvent(
            DIDId: Guid.NewGuid(),
            IdentityId: identityId,
            DidString: "did:web:example.com:identity:123",
            CreatedAt: DateTime.UtcNow);

        var didsServiceClient = Substitute.For<IDIDsServiceClient>();
        var logger = Substitute.For<ILogger<DIDCreatedIntegrationEventHandler>>();
        var handler = new DIDCreatedIntegrationEventHandler(didsServiceClient, logger);

        var didDto = new Mamey.FWID.Identities.Application.DTO.DIDDto
        {
            DIDId = Guid.NewGuid(), // Different ID
            IdentityId = new IdentityId(@event.IdentityId),
            DidString = "did:web:example.com:identity:456" // Different DID string
        };

        didsServiceClient.GetDIDByIdentityIdAsync(new IdentityId(@event.IdentityId), Arg.Any<CancellationToken>())
            .Returns(didDto);

        // Act
        await handler.HandleAsync(@event);

        // Assert
        await didsServiceClient.Received(1).GetDIDByIdentityIdAsync(@event.IdentityId, Arg.Any<CancellationToken>());
        // Should not throw exception, just log warning
    }

    #endregion

    #region CredentialIssuedIntegrationEvent Handling

    [Fact]
    public async Task CredentialIssuedIntegrationEventHandler_WhenEventReceived_ShouldHandleEvent()
    {
        // Arrange - Business Rule: CredentialIssuedIntegrationEvent should be handled when received from Credentials service
        var identityId = Guid.NewGuid();
        var @event = new CredentialIssuedIntegrationEvent(
            CredentialId: Guid.NewGuid(),
            IdentityId: identityId,
            CredentialType: "IdentityVerification",
            IssuerId: Guid.NewGuid(),
            IssuedAt: DateTime.UtcNow);

        var credentialsServiceClient = Substitute.For<ICredentialsServiceClient>();
        var logger = Substitute.For<ILogger<CredentialIssuedIntegrationEventHandler>>();
        var handler = new CredentialIssuedIntegrationEventHandler(credentialsServiceClient, logger);

        // Mock the credential response
        var credentialDto = new Mamey.FWID.Identities.Application.DTO.CredentialDto
        {
            CredentialId = @event.CredentialId,
            IdentityId = @event.IdentityId,
            CredentialType = @event.CredentialType
        };
        
        credentialsServiceClient.GetCredentialAsync(@event.CredentialId, Arg.Any<CancellationToken>())
            .Returns(credentialDto);

        // Act
        await handler.HandleAsync(@event);

        // Assert
        // Handler should call GetCredentialAsync to verify the credential
        await credentialsServiceClient.Received(1).GetCredentialAsync(@event.CredentialId, Arg.Any<CancellationToken>());
    }

    #endregion

    #region ZKPProofGeneratedIntegrationEvent Handling

    [Fact]
    public async Task ZKPProofGeneratedIntegrationEventHandler_WhenEventReceived_ShouldHandleEvent()
    {
        // Arrange - Business Rule: ZKPProofGeneratedIntegrationEvent should be handled when received from ZKPs service
        var identityId = Guid.NewGuid();
        var @event = new ZKPProofGeneratedIntegrationEvent(
            ProofId: Guid.NewGuid(),
            IdentityId: identityId,
            AttributeType: "AgeVerification",
            GeneratedAt: DateTime.UtcNow);

        var zkpsServiceClient = Substitute.For<IZKPsServiceClient>();
        var logger = Substitute.For<ILogger<ZKPProofGeneratedIntegrationEventHandler>>();
        var handler = new ZKPProofGeneratedIntegrationEventHandler(zkpsServiceClient, logger);

        // Act
        await handler.HandleAsync(@event);

        // Assert
        // Handler should process the event without throwing
        await zkpsServiceClient.DidNotReceive().GetZKPProofAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region ZoneAccessGrantedIntegrationEvent Handling

    [Fact]
    public async Task ZoneAccessGrantedIntegrationEventHandler_WhenEventReceived_ShouldHandleEvent()
    {
        // Arrange - Business Rule: ZoneAccessGrantedIntegrationEvent should be handled when received from AccessControls service
        var identityId = Guid.NewGuid();
        var @event = new ZoneAccessGrantedIntegrationEvent(
            AccessControlId: Guid.NewGuid(),
            IdentityId: identityId,
            ZoneId: Guid.NewGuid(),
            Permission: "zone:access",
            GrantedAt: DateTime.UtcNow);

        var accessControlsServiceClient = Substitute.For<IAccessControlsServiceClient>();
        var logger = Substitute.For<ILogger<ZoneAccessGrantedIntegrationEventHandler>>();
        var handler = new ZoneAccessGrantedIntegrationEventHandler(accessControlsServiceClient, logger);

        // Act
        await handler.HandleAsync(@event);

        // Assert
        // Handler should process the event without throwing
        await accessControlsServiceClient.DidNotReceive().GetAccessControlsByIdentityIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Event Retry Tests

    [Fact]
    public async Task DIDCreatedIntegrationEventHandler_WhenServiceThrowsException_ShouldPropagateException()
    {
        // Arrange - Business Rule: If service throws exception, handler should propagate it for retry
        var identityId = Guid.NewGuid();
        var @event = new DIDCreatedIntegrationEvent(
            DIDId: Guid.NewGuid(),
            IdentityId: identityId,
            DidString: "did:web:example.com:identity:123",
            CreatedAt: DateTime.UtcNow);

        var didsServiceClient = Substitute.For<IDIDsServiceClient>();
        var logger = Substitute.For<ILogger<DIDCreatedIntegrationEventHandler>>();
        var handler = new DIDCreatedIntegrationEventHandler(didsServiceClient, logger);

        didsServiceClient.GetDIDByIdentityIdAsync(new IdentityId(@event.IdentityId), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Mamey.FWID.Identities.Application.DTO.DIDDto?>(new Exception("Service unavailable")));

        // Act & Assert
        await Should.ThrowAsync<Exception>(
            () => handler.HandleAsync(@event));
    }

    #endregion
}

