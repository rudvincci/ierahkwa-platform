using Mamey.CQRS.Events;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Events;

internal record UserMultiFactorDisabled(UserId UserId, DateTime DisabledAt) : IEvent;
