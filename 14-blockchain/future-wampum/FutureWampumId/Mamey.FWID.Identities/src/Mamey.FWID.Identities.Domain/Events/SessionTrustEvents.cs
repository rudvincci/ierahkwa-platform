using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Event raised when a session trust score changes.
/// </summary>
internal record SessionTrustScoreChanged(
    SessionId SessionId,
    int OldScore,
    int NewScore,
    string Reason) : IDomainEvent;

/// <summary>
/// Event raised when suspicious activity is detected on a session.
/// </summary>
internal record SessionSuspiciousActivityDetected(
    SessionId SessionId,
    string Activity,
    int Severity) : IDomainEvent;
