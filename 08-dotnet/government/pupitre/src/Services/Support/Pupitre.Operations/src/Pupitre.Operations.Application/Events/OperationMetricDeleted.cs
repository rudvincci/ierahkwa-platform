using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Operations.Application.Events;

[Contract]
internal record OperationMetricDeleted(Guid OperationMetricId) : IEvent;


