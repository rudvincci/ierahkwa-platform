using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when an identity signs out.
/// </summary>
internal record IdentitySignedOut(IdentityId IdentityId, DateTime SignedOutAt) : IDomainEvent;

