using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Accessibility.Application.Events.Rejected;

[Contract]
internal record DeleteAccessProfileRejected(Guid AccessProfileId, string Reason, string Code) : IRejectedEvent;