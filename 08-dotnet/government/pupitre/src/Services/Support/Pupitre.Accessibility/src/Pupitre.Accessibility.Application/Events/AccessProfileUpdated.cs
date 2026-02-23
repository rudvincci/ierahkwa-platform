using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Accessibility.Application.Events;

[Contract]
internal record AccessProfileUpdated(Guid AccessProfileId) : IEvent;


