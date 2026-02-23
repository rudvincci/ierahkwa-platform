using Mamey.CQRS;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Events;

internal record UserTwoFactorEnabled(User User) : IDomainEvent;
