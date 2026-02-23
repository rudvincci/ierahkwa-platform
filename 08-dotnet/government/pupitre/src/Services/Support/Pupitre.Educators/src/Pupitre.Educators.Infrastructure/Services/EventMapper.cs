using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.Educators.Application.Events;
using Pupitre.Educators.Domain.Events;

namespace Pupitre.Educators.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // Educator
            EducatorCreated e => null, // Event published thru handler
            EducatorModified e => new EducatorUpdated(e.Educator.Id),
            EducatorRemoved e => new EducatorDeleted(e.Educator.Id),
            EducatorBlockchainRegistered e => new EducatorCredentialIssued(e.EducatorId, e.IdentityId, e.LedgerTransactionId),
            _ => null
        };
}

