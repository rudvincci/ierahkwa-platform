using Mamey.CQRS;
using Mamey.Government.Identity.Domain.Entities;

namespace Mamey.Government.Identity.Domain.Events;

internal record MfaMethodEnabled(MultiFactorAuth MultiFactorAuth, MfaMethod Method) : IDomainEvent;
