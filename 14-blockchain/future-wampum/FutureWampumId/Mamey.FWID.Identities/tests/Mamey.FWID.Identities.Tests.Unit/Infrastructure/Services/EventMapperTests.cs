using System;
using System.Linq;
using Mamey.CQRS;
using Mamey.FWID.Identities.Application.Events;
using Mamey.FWID.Identities.Application.Events.Integration;
using Mamey.FWID.Identities.Application.Events.Integration.Auth;
using Mamey.FWID.Identities.Application.Events.Integration.Confirmations;
using Mamey.FWID.Identities.Application.Events.Integration.Mfa;
using Mamey.FWID.Identities.Application.Events.Integration.Permissions;
using Mamey.FWID.Identities.Application.Events.Integration.Roles;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Infrastructure.Services;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Infrastructure.Services;

public class EventMapperTests
{
    private readonly EventMapper _mapper;

    public EventMapperTests()
    {
        _mapper = new EventMapper();
    }

    [Fact]
    public void Map_WithIdentityCreated_ShouldReturnNull()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var @event = new IdentityCreated(identityId, new Mamey.Types.Name("John", "Doe", "M."), DateTime.UtcNow, "zone-001");

        // Act
        var result = _mapper.Map(@event);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void Map_WithIdentityModified_ShouldReturnIdentityUpdated()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var identity = new Identity(
            identityId,
            new Mamey.Types.Name("John", "Doe", "M."),
            new Mamey.FWID.Identities.Domain.ValueObjects.PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            new Mamey.FWID.Identities.Domain.ValueObjects.ContactInformation(
                new Mamey.Types.Email("john.doe@example.com"),
                new Mamey.Types.Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Mamey.Types.Phone> { new Mamey.Types.Phone("1", "5551234567", null, Mamey.Types.Phone.PhoneType.Mobile) }
            ),
            new Mamey.FWID.Identities.Domain.ValueObjects.BiometricData(Mamey.FWID.Identities.Domain.ValueObjects.BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            "zone-001",
            null);
        var @event = new IdentityModified(identity);

        // Act
        var result = _mapper.Map(@event);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<IdentityUpdated>();
        var identityUpdated = result as IdentityUpdated;
        identityUpdated!.IdentityId.ShouldBe(identityId.Value);
    }

    [Fact]
    public void Map_WithIdentityRemoved_ShouldReturnIdentityDeleted()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var identity = new Identity(
            identityId,
            new Mamey.Types.Name("John", "Doe", "M."),
            new Mamey.FWID.Identities.Domain.ValueObjects.PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            new Mamey.FWID.Identities.Domain.ValueObjects.ContactInformation(
                new Mamey.Types.Email("john.doe@example.com"),
                new Mamey.Types.Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Mamey.Types.Phone> { new Mamey.Types.Phone("1", "5551234567", null, Mamey.Types.Phone.PhoneType.Mobile) }
            ),
            new Mamey.FWID.Identities.Domain.ValueObjects.BiometricData(Mamey.FWID.Identities.Domain.ValueObjects.BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            "zone-001",
            null);
        var @event = new IdentityRemoved(identity);

        // Act
        var result = _mapper.Map(@event);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<IdentityDeleted>();
        var identityDeleted = result as IdentityDeleted;
        identityDeleted!.IdentityId.ShouldBe(identityId.Value);
    }

    [Fact]
    public void Map_WithUnknownEvent_ShouldReturnNull()
    {
        // Arrange
        var @event = new UnknownDomainEvent();

        // Act
        var result = _mapper.Map(@event);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void MapAll_WithMultipleEvents_ShouldMapAllEvents()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var identity = new Identity(
            identityId,
            new Mamey.Types.Name("John", "Doe", "M."),
            new Mamey.FWID.Identities.Domain.ValueObjects.PersonalDetails(new DateTime(1990, 1, 1), "New York, NY", "Male", "Wolf Clan"),
            new Mamey.FWID.Identities.Domain.ValueObjects.ContactInformation(
                new Mamey.Types.Email("john.doe@example.com"),
                new Mamey.Types.Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Mamey.Types.Phone> { new Mamey.Types.Phone("1", "5551234567", null, Mamey.Types.Phone.PhoneType.Mobile) }
            ),
            new Mamey.FWID.Identities.Domain.ValueObjects.BiometricData(Mamey.FWID.Identities.Domain.ValueObjects.BiometricType.Fingerprint, new byte[] { 1, 2, 3 }, Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(new byte[] { 1, 2, 3 })).ToLowerInvariant()),
            "zone-001",
            null);

        var events = new List<IDomainEvent>
        {
            new IdentityCreated(identityId, new Mamey.Types.Name("John", "Doe", "M."), DateTime.UtcNow, "zone-001"),
            new IdentityModified(identity),
            new IdentityRemoved(identity)
        };

        // Act
        var results = _mapper.MapAll(events).ToList();

        // Assert
        results.Count.ShouldBe(3);
        results[0].ShouldBeNull(); // IdentityCreated maps to null
        results[1].ShouldBeOfType<IdentityUpdated>();
        results[2].ShouldBeOfType<IdentityDeleted>();
    }

    [Fact]
    public void Map_WithSessionCreated_ShouldReturnSignInIntegrationEvent()
    {
        // Arrange
        var sessionId = new SessionId();
        var identityId = new IdentityId(Guid.NewGuid());
        var @event = new SessionCreated(sessionId, identityId, "192.168.1.1", "Mozilla/5.0", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));

        // Act
        var result = _mapper.Map(@event);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<SignInIntegrationEvent>();
        var signInEvent = result as SignInIntegrationEvent;
        signInEvent!.IdentityId.ShouldBe(identityId.Value);
        signInEvent.SessionId.ShouldBe(sessionId.Value);
    }

    [Fact]
    public void Map_WithSessionRevoked_ShouldReturnSignOutIntegrationEvent()
    {
        // Arrange
        var sessionId = new SessionId();
        var identityId = new IdentityId(Guid.NewGuid());
        var @event = new SessionRevoked(sessionId, identityId, DateTime.UtcNow);

        // Act
        var result = _mapper.Map(@event);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<SignOutIntegrationEvent>();
        var signOutEvent = result as SignOutIntegrationEvent;
        signOutEvent!.IdentityId.ShouldBe(identityId.Value);
        signOutEvent.SessionId.ShouldBe(sessionId.Value);
    }

    [Fact]
    public void Map_WithMfaEnabled_ShouldReturnMfaEnabledIntegrationEvent()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var @event = new MfaEnabled(identityId, MfaMethod.Totp, DateTime.UtcNow);

        // Act
        var result = _mapper.Map(@event);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<MfaEnabledIntegrationEvent>();
        var mfaEvent = result as MfaEnabledIntegrationEvent;
        mfaEvent!.IdentityId.ShouldBe(identityId.Value);
        mfaEvent.MfaMethod.ShouldBe("Totp");
    }

    [Fact]
    public void Map_WithEmailConfirmed_ShouldReturnEmailConfirmedIntegrationEvent()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var @event = new EmailConfirmed(identityId, DateTime.UtcNow);

        // Act
        var result = _mapper.Map(@event);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<EmailConfirmedIntegrationEvent>();
        var emailEvent = result as EmailConfirmedIntegrationEvent;
        emailEvent!.IdentityId.ShouldBe(identityId.Value);
    }

    [Fact]
    public void Map_WithPermissionAssigned_ShouldReturnPermissionAssignedIntegrationEvent()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var permissionId = Guid.NewGuid();
        var @event = new PermissionAssigned(identityId, permissionId, DateTime.UtcNow);

        // Act
        var result = _mapper.Map(@event);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<PermissionAssignedIntegrationEvent>();
        var permissionEvent = result as PermissionAssignedIntegrationEvent;
        permissionEvent!.IdentityId.ShouldBe(identityId.Value);
        permissionEvent.PermissionId.ShouldBe(permissionId);
    }

    [Fact]
    public void Map_WithRoleAssigned_ShouldReturnRoleAssignedIntegrationEvent()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var roleId = Guid.NewGuid();
        var @event = new RoleAssigned(identityId, roleId, DateTime.UtcNow);

        // Act
        var result = _mapper.Map(@event);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<RoleAssignedIntegrationEvent>();
        var roleEvent = result as RoleAssignedIntegrationEvent;
        roleEvent!.IdentityId.ShouldBe(identityId.Value);
        roleEvent.RoleId.ShouldBe(roleId);
    }

    private class UnknownDomainEvent : IDomainEvent
    {
    }
}

