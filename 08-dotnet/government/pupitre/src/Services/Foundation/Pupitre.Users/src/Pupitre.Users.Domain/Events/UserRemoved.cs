using Mamey.CQRS;
using Pupitre.Users.Domain.Entities;

namespace Pupitre.Users.Domain.Events;

internal record UserRemoved(User User) : IDomainEvent;