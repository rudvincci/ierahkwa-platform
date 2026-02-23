using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when an identity is verified.
/// </summary>
[Contract]
internal record IdentityVerified(IdentityId IdentityId, DateTime VerifiedAt, double MatchScore) : IDomainEvent;

