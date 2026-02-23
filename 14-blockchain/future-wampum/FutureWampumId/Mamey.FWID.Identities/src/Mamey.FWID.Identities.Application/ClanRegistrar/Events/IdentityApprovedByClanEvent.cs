using Mamey.FWID.Identities.Application.ClanRegistrar.Models;

namespace Mamey.FWID.Identities.Application.ClanRegistrar.Events;

/// <summary>
/// Event raised when an identity is approved by a clan registrar.
/// </summary>
public record IdentityApprovedByClanEvent(
    Guid ApprovalId,
    Guid IdentityId,
    Guid RegistrarId,
    string Zone,
    string? ClanName,
    DateTime ApprovedAt
) : IIntegrationEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}

/// <summary>
/// Event raised when an identity registration is rejected.
/// </summary>
public record IdentityRejectedByClanEvent(
    Guid ApprovalId,
    Guid IdentityId,
    Guid RegistrarId,
    string Zone,
    string Reason,
    DateTime RejectedAt
) : IIntegrationEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}

/// <summary>
/// Event raised when an approval is escalated to elder council.
/// </summary>
public record ApprovalEscalatedToCouncilEvent(
    Guid ApprovalId,
    Guid IdentityId,
    Guid FromRegistrarId,
    Guid CouncilId,
    string Reason,
    DateTime EscalatedAt
) : IIntegrationEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}

/// <summary>
/// Event raised when a new registrar is appointed.
/// </summary>
public record ClanRegistrarAppointedEvent(
    Guid RegistrarId,
    Guid IdentityId,
    string DID,
    RegistrarType Type,
    string Zone,
    string? ClanName,
    Guid? AppointedBy,
    DateTime AppointedAt
) : IIntegrationEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}

/// <summary>
/// Event raised when authority is delegated between registrars.
/// </summary>
public record RegistrarAuthorityDelegatedEvent(
    Guid FromRegistrarId,
    Guid ToRegistrarId,
    string Zone,
    RegistrarScope Scope,
    DateTime DelegatedAt
) : IIntegrationEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}

/// <summary>
/// Marker interface for integration events.
/// </summary>
public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTime Timestamp { get; }
}
