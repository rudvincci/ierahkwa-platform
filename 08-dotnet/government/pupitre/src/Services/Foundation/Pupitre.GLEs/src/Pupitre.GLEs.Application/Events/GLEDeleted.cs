using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.GLEs.Application.Events;

[Contract]
internal record GLEDeleted(Guid GLEId) : IEvent;


