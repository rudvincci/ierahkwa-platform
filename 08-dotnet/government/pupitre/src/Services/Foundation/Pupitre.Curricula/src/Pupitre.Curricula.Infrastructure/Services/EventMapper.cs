using Mamey.CQRS;
using Mamey.CQRS.Events;
using Pupitre.Curricula.Application.Events;
using Pupitre.Curricula.Domain.Events;

namespace Pupitre.Curricula.Infrastructure.Services;

internal sealed class EventMapper : IEventMapper
{
    public IEnumerable<IEvent?> MapAll(IEnumerable<IDomainEvent> events)
        => events.Select(Map);

    public IEvent? Map(IDomainEvent @event)
        => @event switch
        {
            // Curriculum
            CurriculumCreated e => null, // Event published thru handler
            CurriculumModified e => new CurriculumUpdated(e.Curriculum.Id),
            CurriculumRemoved e => new CurriculumDeleted(e.Curriculum.Id),
            CurriculumBlockchainRegistered e => new CurriculumCredentialIssued(e.CurriculumId, e.IdentityId, e.LedgerTransactionId),
            _ => null
        };
}

