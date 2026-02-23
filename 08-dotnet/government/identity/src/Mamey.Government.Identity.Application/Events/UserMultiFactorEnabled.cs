using Mamey.CQRS.Events;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Events;

internal record UserMultiFactorEnabled(UserId UserId, IEnumerable<MfaMethod> EnabledMethods, int RequiredMethods, DateTime EnabledAt) : IEvent;