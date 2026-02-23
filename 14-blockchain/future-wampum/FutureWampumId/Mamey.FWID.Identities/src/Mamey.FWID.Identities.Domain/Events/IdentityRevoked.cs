using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when an identity is revoked.
/// </summary>
[Contract]
internal record IdentityRevoked(IdentityId IdentityId, string Reason, DateTime RevokedAt, Guid? RevokedBy) : IDomainEvent;

