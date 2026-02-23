using Mamey.CQRS;
using Pupitre.Operations.Domain.Entities;

namespace Pupitre.Operations.Domain.Events;

internal record OperationMetricRemoved(OperationMetric OperationMetric) : IDomainEvent;