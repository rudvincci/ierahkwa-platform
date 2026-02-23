using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when a sign-in attempt fails.
/// </summary>
internal record SignInFailed(IdentityId IdentityId, int FailedAttempts, DateTime FailedAt) : IDomainEvent;

