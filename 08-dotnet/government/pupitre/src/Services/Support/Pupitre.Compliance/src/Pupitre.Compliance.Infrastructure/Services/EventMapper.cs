using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.Compliance.Application.Events;
using Pupitre.Compliance.Domain.Events;

namespace Pupitre.Compliance.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // ComplianceRecord
            ComplianceRecordCreated e => null, // Event published thru handler
            ComplianceRecordModified e => new ComplianceRecordUpdated(e.ComplianceRecord.Id),
            ComplianceRecordRemoved e => new ComplianceRecordDeleted(e.ComplianceRecord.Id),
            _ => null
        };
}

