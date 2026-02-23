using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Curricula.Application.Events.Rejected;

[Contract]
internal record UpdateCurriculumRejected(Guid CurriculumId, string Reason, string Code) : IRejectedEvent;
