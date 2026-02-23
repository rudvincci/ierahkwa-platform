using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Aftercare.Application.Events.Rejected;

[Contract]
internal record DeleteAftercarePlanRejected(Guid AftercarePlanId, string Reason, string Code) : IRejectedEvent;