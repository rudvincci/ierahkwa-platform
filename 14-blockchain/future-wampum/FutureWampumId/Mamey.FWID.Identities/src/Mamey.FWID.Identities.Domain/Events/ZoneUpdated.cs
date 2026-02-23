using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when an identity's zone is updated.
/// </summary>
[Contract]
internal record ZoneUpdated(IdentityId IdentityId, string? OldZone, string? NewZone, DateTime UpdatedAt) : IDomainEvent;

