using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Educators.Application.Events;

[Contract]
internal record EducatorUpdated(Guid EducatorId) : IEvent;


