using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when an MFA challenge is successfully verified.
/// </summary>
internal record MfaVerified(IdentityId IdentityId, MfaMethod Method, DateTime VerifiedAt) : IDomainEvent;

