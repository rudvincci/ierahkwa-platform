using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Parents.Application.Events;

[Contract]
internal record ParentDeleted(Guid ParentId) : IEvent;


