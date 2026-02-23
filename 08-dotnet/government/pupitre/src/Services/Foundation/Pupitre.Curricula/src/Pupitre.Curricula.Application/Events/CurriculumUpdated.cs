using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Curricula.Application.Events;

[Contract]
internal record CurriculumUpdated(Guid CurriculumId) : IEvent;


