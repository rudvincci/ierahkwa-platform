using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.Fundraising.Application.Events;
using Pupitre.Fundraising.Domain.Events;

namespace Pupitre.Fundraising.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // Campaign
            CampaignCreated e => null, // Event published thru handler
            CampaignModified e => new CampaignUpdated(e.Campaign.Id),
            CampaignRemoved e => new CampaignDeleted(e.Campaign.Id),
            _ => null
        };
}

