using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when a session is created.
/// </summary>
internal record SessionCreated(
    SessionId SessionId, 
    IdentityId IdentityId, 
    string? IpAddress, 
    string? UserAgent,
    DateTime CreatedAt, 
    DateTime ExpiresAt) : IDomainEvent;

