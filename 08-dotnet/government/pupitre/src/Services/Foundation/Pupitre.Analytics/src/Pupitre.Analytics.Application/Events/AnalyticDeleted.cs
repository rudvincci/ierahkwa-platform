using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Analytics.Application.Events;

[Contract]
internal record AnalyticDeleted(Guid AnalyticId) : IEvent;


