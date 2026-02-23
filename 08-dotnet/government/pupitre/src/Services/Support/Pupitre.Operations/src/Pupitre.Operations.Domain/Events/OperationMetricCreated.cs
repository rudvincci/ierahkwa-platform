using Mamey.CQRS;
using Pupitre.Operations.Domain.Entities;

namespace Pupitre.Operations.Domain.Events;

internal record OperationMetricCreated(OperationMetric OperationMetric) : IDomainEvent;

