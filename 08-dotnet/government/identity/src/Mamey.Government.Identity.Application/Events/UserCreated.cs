using Mamey.CQRS.Events;
using Mamey.Government.Identity.Domain.Entities;

namespace Mamey.Government.Identity.Application.Events;

internal record UserCreated(User User) : IEvent;