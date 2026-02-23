using Mamey.CQRS;
using Mamey.Government.Identity.Domain.Entities;

namespace Mamey.Government.Identity.Domain.Events;

internal record UserLoginFailed(User User, int FailedAttempts) : IDomainEvent;
