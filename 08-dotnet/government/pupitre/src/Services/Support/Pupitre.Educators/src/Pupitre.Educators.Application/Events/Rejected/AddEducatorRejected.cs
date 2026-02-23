using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Educators.Application.Events.Rejected;

[Contract]
internal record AddEducatorRejected(Guid EducatorId, string Reason, string Code) : IRejectedEvent;
