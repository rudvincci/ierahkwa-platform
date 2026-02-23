using Mamey.CQRS.Events;
using Mamey.Government.Identity.Domain.Entities;

namespace Mamey.Government.Identity.Application.Events;

internal record UserTwoFactorEnabled(User User, TwoFactorAuth TwoFactorAuth) : IEvent;