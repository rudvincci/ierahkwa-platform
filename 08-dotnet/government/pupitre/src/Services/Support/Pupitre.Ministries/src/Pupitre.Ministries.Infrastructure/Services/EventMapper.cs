using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.Ministries.Application.Events;
using Pupitre.Ministries.Domain.Events;

namespace Pupitre.Ministries.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // MinistryData
            MinistryDataCreated e => null, // Event published thru handler
            MinistryDataModified e => new MinistryDataUpdated(e.MinistryData.Id),
            MinistryDataRemoved e => new MinistryDataDeleted(e.MinistryData.Id),
            MinistryDataBlockchainRegistered e => new MinistryCredentialIssued(e.MinistryDataId, e.IdentityId, e.LedgerTransactionId),
            _ => null
        };
}

