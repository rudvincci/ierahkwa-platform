using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.GLEs.Application.Events.Rejected;

[Contract]
internal record AddGLERejected(Guid GLEId, string Reason, string Code) : IRejectedEvent;
