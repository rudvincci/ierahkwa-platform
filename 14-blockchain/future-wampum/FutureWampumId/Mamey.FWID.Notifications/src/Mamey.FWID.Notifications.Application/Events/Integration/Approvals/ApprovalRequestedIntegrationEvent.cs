using Mamey.CQRS.Events;

namespace Mamey.FWID.Notifications.Application.Events.Integration.Approvals;

/// <summary>
/// Integration event for approval requests.
/// </summary>
public record ApprovalRequestedIntegrationEvent : IEvent
{
    public Guid ApprovalId { get; init; }
    public string ApprovalType { get; init; } = null!;
    public string RequesterName { get; init; } = null!;
    public Guid RequesterIdentityId { get; init; }
    public string Description { get; init; } = null!;
    public DateTime RequestedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public Guid? ApproverIdentityId { get; init; }
    public string? ApproverEmail { get; init; }
}

/// <summary>
/// Integration event for credential expiring notification.
/// </summary>
public record CredentialExpiringIntegrationEvent : IEvent
{
    public Guid CredentialId { get; init; }
    public string CredentialType { get; init; } = null!;
    public Guid HolderIdentityId { get; init; }
    public string? HolderEmail { get; init; }
    public DateTime ExpiresAt { get; init; }
    public int DaysUntilExpiration { get; init; }
}

/// <summary>
/// Integration event for security alerts.
/// </summary>
public record SecurityAlertIntegrationEvent : IEvent
{
    public Guid IdentityId { get; init; }
    public string AlertType { get; init; } = null!;
    public string AlertLevel { get; init; } = null!; // Low, Medium, High, Critical
    public string Description { get; init; } = null!;
    public DateTime OccurredAt { get; init; }
    public string? IpAddress { get; init; }
    public string? Location { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? ActionRequired { get; init; }
}

/// <summary>
/// Integration event for registration completed.
/// </summary>
public record RegistrationCompletedIntegrationEvent : IEvent
{
    public Guid IdentityId { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string DID { get; init; } = null!;
    public Guid CredentialId { get; init; }
    public string Zone { get; init; } = null!;
    public DateTime CompletedAt { get; init; }
}
