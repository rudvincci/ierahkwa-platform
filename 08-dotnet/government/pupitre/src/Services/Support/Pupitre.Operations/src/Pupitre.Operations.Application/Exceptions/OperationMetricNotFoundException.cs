using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Operations.Domain.Entities;

namespace Pupitre.Operations.Application.Exceptions;

internal class OperationMetricNotFoundException : MameyException
{
    public OperationMetricNotFoundException(OperationMetricId operationmetricId)
        : base($"OperationMetric with ID: '{operationmetricId.Value}' was not found.")
        => OperationMetricId = operationmetricId;

    public OperationMetricId OperationMetricId { get; }
}

