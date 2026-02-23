using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Accessibility.Application.Events;

[Contract]
internal record AccessProfileDeleted(Guid AccessProfileId) : IEvent;


