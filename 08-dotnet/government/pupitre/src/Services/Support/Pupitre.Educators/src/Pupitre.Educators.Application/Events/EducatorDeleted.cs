using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Educators.Application.Events;

[Contract]
internal record EducatorDeleted(Guid EducatorId) : IEvent;


