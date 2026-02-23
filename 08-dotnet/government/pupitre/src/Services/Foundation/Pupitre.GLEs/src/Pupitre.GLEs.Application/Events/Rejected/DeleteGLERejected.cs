using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.GLEs.Application.Events.Rejected;

[Contract]
internal record DeleteGLERejected(Guid GLEId, string Reason, string Code) : IRejectedEvent;