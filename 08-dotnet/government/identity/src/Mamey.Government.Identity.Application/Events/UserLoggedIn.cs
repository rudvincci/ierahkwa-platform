using Mamey.CQRS.Events;
using Mamey.Government.Identity.Domain.Entities;

namespace Mamey.Government.Identity.Application.Events;

internal record UserLoggedIn(User User, string IpAddress) : IEvent;
