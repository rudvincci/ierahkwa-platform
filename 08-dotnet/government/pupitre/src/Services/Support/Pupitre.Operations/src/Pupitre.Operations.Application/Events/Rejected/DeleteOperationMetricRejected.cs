using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Operations.Application.Events.Rejected;

[Contract]
internal record DeleteOperationMetricRejected(Guid OperationMetricId, string Reason, string Code) : IRejectedEvent;