using Mamey.CQRS;
using Mamey.Government.Identity.Domain.Entities;

namespace Mamey.Government.Identity.Domain.Events;

internal record MfaMethodDisabled(MultiFactorAuth MultiFactorAuth, MfaMethod Method) : IDomainEvent;
