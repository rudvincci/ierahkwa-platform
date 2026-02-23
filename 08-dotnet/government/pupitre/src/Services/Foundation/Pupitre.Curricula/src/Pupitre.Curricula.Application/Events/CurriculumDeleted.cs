using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Curricula.Application.Events;

[Contract]
internal record CurriculumDeleted(Guid CurriculumId) : IEvent;


