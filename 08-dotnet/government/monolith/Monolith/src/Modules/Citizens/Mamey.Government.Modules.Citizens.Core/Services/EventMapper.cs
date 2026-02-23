using Mamey.CQRS;
using Mamey.CQRS.Events;
using Mamey.Government.Modules.Citizens.Core.Domain.Events;
using Mamey.Government.Modules.Citizens.Core.Events;

namespace Mamey.Government.Modules.Citizens.Core.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            CitizenCreated e => new CitizenCreatedEvent(
                e.CitizenId.Value,
                e.TenantId.Value,
                e.CitizenName.FirstName,
                e.CitizenName.LastName,  
                e.Status.ToString(),
                e.ApplicationId == default ? null : e.ApplicationId),
            
            CitizenModified e => new CitizenUpdatedEvent(e.Citizen.Id.Value),
            
            CitizenStatusChanged e => new CitizenStatusChangedEvent(
                e.CitizenId.Value,
                e.OldStatus.ToString(),
                e.NewStatus.ToString(),
                e.Reason),
            
            CitizenDeactivated e => new CitizenDeactivatedEvent(
                e.CitizenId.Value,
                e.Reason),
            
            _ => null
        };
}
