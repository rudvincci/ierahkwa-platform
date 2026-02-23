using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when an identity signs in.
/// </summary>
internal record IdentitySignedIn(IdentityId IdentityId, DateTime SignedInAt) : IDomainEvent;

