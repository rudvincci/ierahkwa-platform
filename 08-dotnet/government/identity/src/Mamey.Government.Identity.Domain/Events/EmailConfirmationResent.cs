using Mamey.CQRS;
using Mamey.Government.Identity.Domain.Entities;

namespace Mamey.Government.Identity.Domain.Events;

internal record EmailConfirmationResent(EmailConfirmation EmailConfirmation) : IDomainEvent;
