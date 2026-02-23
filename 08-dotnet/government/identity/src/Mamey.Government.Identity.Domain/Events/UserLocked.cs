using Mamey.CQRS;
using Mamey.Government.Identity.Domain.Entities;

namespace Mamey.Government.Identity.Domain.Events;

internal record UserLocked(User User, DateTime LockedUntil) : IDomainEvent;
