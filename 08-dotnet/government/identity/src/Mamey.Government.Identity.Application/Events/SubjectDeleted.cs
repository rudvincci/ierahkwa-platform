using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.Government.Identity.Application.Events;

[Contract]
internal record SubjectDeleted(Guid SubjectId) : IEvent;


