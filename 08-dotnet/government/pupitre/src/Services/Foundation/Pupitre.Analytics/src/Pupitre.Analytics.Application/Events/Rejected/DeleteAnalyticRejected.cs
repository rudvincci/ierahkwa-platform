using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Analytics.Application.Events.Rejected;

[Contract]
internal record DeleteAnalyticRejected(Guid AnalyticId, string Reason, string Code) : IRejectedEvent;