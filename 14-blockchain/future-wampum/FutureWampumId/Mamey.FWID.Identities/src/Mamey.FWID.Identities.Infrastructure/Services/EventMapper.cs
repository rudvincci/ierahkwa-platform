using Mamey.CQRS;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Events;
using Mamey.FWID.Identities.Application.Events.Integration.Auth;
using Mamey.FWID.Identities.Application.Events.Integration.Confirmations;
using Mamey.FWID.Identities.Application.Events.Integration.Mfa;
using Mamey.FWID.Identities.Application.Events.Integration.Permissions;
using Mamey.FWID.Identities.Application.Events.Integration.Roles;
using Mamey.FWID.Identities.Domain.Events;

namespace Mamey.FWID.Identities.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // Identity
            IdentityCreated e => null, // Event published thru handler
            IdentityModified e => new IdentityUpdated(e.Identity.Id),
            IdentityRemoved e => new IdentityDeleted(e.Identity.Id),
            
            // Authentication - Session events
            SessionCreated e => new SignInIntegrationEvent(
                e.IdentityId.Value,
                e.SessionId.Value,
                "JWT", // Authentication method - could be enhanced to detect actual method
                e.IpAddress,
                e.UserAgent,
                e.CreatedAt),
            SessionRevoked e => new SignOutIntegrationEvent(
                e.IdentityId.Value,
                e.SessionId.Value,
                e.RevokedAt),
            // Note: TokenRefreshed would need to be published from RefreshTokenHandler directly
            
            // MFA
            MfaEnabled e => new MfaEnabledIntegrationEvent(
                e.IdentityId.Value,
                e.Method.ToString(),
                e.EnabledAt),
            MfaDisabled e => new MfaDisabledIntegrationEvent(
                e.IdentityId.Value,
                "Unknown", // Method not in domain event - would need to fetch from MfaConfiguration
                e.DisabledAt),
            MfaVerified e => new MfaVerifiedIntegrationEvent(
                e.IdentityId.Value,
                e.Method.ToString(),
                e.VerifiedAt),
            MfaFailed e => new MfaFailedIntegrationEvent(
                e.IdentityId.Value,
                e.Method.ToString(),
                "Verification failed", // Reason not in domain event
                e.FailedAt),
            
            // Confirmations
            EmailConfirmed e => new EmailConfirmedIntegrationEvent(
                e.IdentityId.Value,
                string.Empty, // Email not in domain event - would need to fetch from identity
                e.ConfirmedAt),
            SmsConfirmed e => new SmsConfirmedIntegrationEvent(
                e.IdentityId.Value,
                string.Empty, // PhoneNumber not in domain event - would need to fetch from identity
                e.ConfirmedAt),
            
            // Permissions
            PermissionAssigned e => new PermissionAssignedIntegrationEvent(
                e.IdentityId.Value,
                e.PermissionId, // Already Guid
                string.Empty, // PermissionName not in domain event - would need to fetch
                e.AssignedAt),
            PermissionRemoved e => new PermissionRemovedIntegrationEvent(
                e.IdentityId.Value,
                e.PermissionId, // Already Guid
                string.Empty, // PermissionName not in domain event - would need to fetch
                e.RemovedAt),
            
            // Roles
            RoleAssigned e => new RoleAssignedIntegrationEvent(
                e.IdentityId.Value,
                e.RoleId, // Already Guid
                string.Empty, // RoleName not in domain event - would need to fetch
                e.AssignedAt),
            RoleRemoved e => new RoleRemovedIntegrationEvent(
                e.IdentityId.Value,
                e.RoleId, // Already Guid
                string.Empty, // RoleName not in domain event - would need to fetch
                e.RemovedAt),
            
            _ => null
        };
}

