using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.Users.Application.Events;
using Pupitre.Users.Domain.Events;

namespace Pupitre.Users.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // User
            UserCreated e => null, // Event published thru handler
            UserModified e => new UserUpdated(e.User.Id),
            UserRemoved e => new UserDeleted(e.User.Id),
            UserBlockchainRegistered e => new UserCredentialIssued(e.UserId, e.IdentityId, e.LedgerTransactionId),
            _ => null
        };
}

