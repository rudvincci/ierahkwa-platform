using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when an MFA challenge verification fails.
/// </summary>
internal record MfaFailed(IdentityId IdentityId, MfaMethod Method, DateTime FailedAt) : IDomainEvent;

