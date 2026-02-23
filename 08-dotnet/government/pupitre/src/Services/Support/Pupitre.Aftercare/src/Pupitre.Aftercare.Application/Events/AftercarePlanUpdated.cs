using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Aftercare.Application.Events;

[Contract]
internal record AftercarePlanUpdated(Guid AftercarePlanId) : IEvent;


